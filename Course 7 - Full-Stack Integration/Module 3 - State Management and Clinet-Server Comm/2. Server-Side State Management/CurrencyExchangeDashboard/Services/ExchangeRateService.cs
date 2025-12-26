using System.Text.Json;
using CurrencyExchangeDashboard.Models;
using CurrencyExchangeDashboard.Services.Interfaces;

namespace CurrencyExchangeDashboard.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _httpClient;
    private readonly ICacheService _cacheService;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private static readonly TimeSpan RatesCacheDuration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan HistoryCacheDuration = TimeSpan.FromHours(1);
    private static readonly TimeSpan CurrenciesCacheDuration = TimeSpan.FromHours(24);

    public bool IsFromCache { get; private set; }
    public DateTime? LastCacheTime { get; private set; }

    public ExchangeRateService(HttpClient httpClient, ICacheService cacheService)
    {
        _httpClient = httpClient;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<ExchangeRate>> GetLatestRatesAsync(
        string baseCurrency,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"rates_{baseCurrency}";

        if (_cacheService.TryGetWithMetadata<List<ExchangeRate>>(cacheKey, out var cached, out var cachedAt)
            && cached is not null)
        {
            IsFromCache = true;
            LastCacheTime = cachedAt;
            return cached;
        }

        IsFromCache = false;
        LastCacheTime = null;

        var response = await _httpClient.GetAsync(
            $"https://api.frankfurter.app/latest?from={baseCurrency}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<FrankfurterLatestResponse>(json, JsonOptions);

        if (data?.Rates is null)
            return [];

        var now = DateTime.UtcNow;
        var rates = data.Rates
            .Select(kvp => new ExchangeRate(kvp.Key, kvp.Value, now))
            .ToList();

        _cacheService.Set(cacheKey, rates, RatesCacheDuration);
        LastCacheTime = now;

        return rates;
    }

    public async Task<IReadOnlyList<HistoricalRate>> GetHistoricalRatesAsync(
        string from,
        string to,
        int days,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"history_{from}_{to}_{days}";

        if (_cacheService.TryGetWithMetadata<List<HistoricalRate>>(cacheKey, out var cached, out _)
            && cached is not null)
        {
            IsFromCache = true;
            return cached;
        }

        IsFromCache = false;

        var endDate = DateTime.UtcNow.Date;
        var startDate = endDate.AddDays(-days);

        var url = $"https://api.frankfurter.app/{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?from={from}&to={to}";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<FrankfurterHistoricalResponse>(json, JsonOptions);

        if (data?.Rates is null)
            return [];

        var rates = data.Rates
            .Select(kvp => new HistoricalRate(
                DateTime.Parse(kvp.Key),
                kvp.Value.TryGetValue(to, out var rate) ? rate : 0))
            .OrderBy(r => r.Date)
            .ToList();

        _cacheService.Set(cacheKey, rates, HistoryCacheDuration);

        return rates;
    }

    public async Task<Dictionary<string, string>> GetAvailableCurrenciesAsync(
        CancellationToken cancellationToken = default)
    {
        const string cacheKey = "currencies";

        if (_cacheService.TryGetWithMetadata<Dictionary<string, string>>(cacheKey, out var cached, out _)
            && cached is not null)
        {
            return cached;
        }

        var response = await _httpClient.GetAsync(
            "https://api.frankfurter.app/currencies",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var currencies = JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions) ?? [];

        _cacheService.Set(cacheKey, currencies, CurrenciesCacheDuration);

        return currencies;
    }

    public async Task<decimal> ConvertAsync(
        string from,
        string to,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        if (from == to)
            return amount;

        var rates = await GetLatestRatesAsync(from, cancellationToken);
        var rate = rates.FirstOrDefault(r => r.Currency == to);

        return rate is not null ? amount * rate.Rate : 0;
    }

    public void ClearRatesCache(string baseCurrency)
    {
        _cacheService.Remove($"rates_{baseCurrency}");
    }

    private sealed class FrankfurterLatestResponse
    {
        public Dictionary<string, decimal>? Rates { get; set; }
    }

    private sealed class FrankfurterHistoricalResponse
    {
        public Dictionary<string, Dictionary<string, decimal>>? Rates { get; set; }
    }
}
