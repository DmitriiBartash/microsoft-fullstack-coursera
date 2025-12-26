using CurrencyExchangeDashboard.Models;

namespace CurrencyExchangeDashboard.Services.Interfaces;

public interface IExchangeRateService
{
    Task<IReadOnlyList<ExchangeRate>> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HistoricalRate>> GetHistoricalRatesAsync(string from, string to, int days, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> GetAvailableCurrenciesAsync(CancellationToken cancellationToken = default);
    Task<decimal> ConvertAsync(string from, string to, decimal amount, CancellationToken cancellationToken = default);
    bool IsFromCache { get; }
    DateTime? LastCacheTime { get; }
    void ClearRatesCache(string baseCurrency);
}
