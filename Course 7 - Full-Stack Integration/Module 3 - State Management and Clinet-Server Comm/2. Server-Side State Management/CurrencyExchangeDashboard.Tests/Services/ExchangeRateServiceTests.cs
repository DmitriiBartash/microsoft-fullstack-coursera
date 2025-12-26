using System.Net;
using CurrencyExchangeDashboard.Models;
using CurrencyExchangeDashboard.Services;
using CurrencyExchangeDashboard.Services.Interfaces;
using Moq;

namespace CurrencyExchangeDashboard.Tests.Services;

public class ExchangeRateServiceTests
{
    private readonly Mock<ICacheService> _cacheServiceMock;

    public ExchangeRateServiceTests()
    {
        _cacheServiceMock = new Mock<ICacheService>();
    }

    private ExchangeRateService CreateService(HttpClient httpClient)
    {
        return new ExchangeRateService(httpClient, _cacheServiceMock.Object);
    }

    private static HttpClient CreateMockHttpClient(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handler = new MockHttpMessageHandler(responseContent, statusCode);
        return new HttpClient(handler);
    }

    [Fact]
    public async Task GetLatestRatesAsync_WhenCached_ReturnsCachedData()
    {
        // Arrange
        var cachedRates = new List<ExchangeRate>
        {
            new("EUR", 0.85m, DateTime.UtcNow),
            new("GBP", 0.73m, DateTime.UtcNow)
        };
        var cachedAt = DateTime.UtcNow.AddMinutes(-2);

        _cacheServiceMock
            .Setup(c => c.TryGetWithMetadata<List<ExchangeRate>>(
                "rates_USD",
                out It.Ref<List<ExchangeRate>?>.IsAny,
                out It.Ref<DateTime?>.IsAny))
            .Callback(new TryGetWithMetadataCallback<List<ExchangeRate>>((string key, out List<ExchangeRate>? value, out DateTime? cached) =>
            {
                value = cachedRates;
                cached = cachedAt;
            }))
            .Returns(true);

        var httpClient = CreateMockHttpClient("{}");
        var sut = CreateService(httpClient);

        // Act
        var result = await sut.GetLatestRatesAsync("USD");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.True(sut.IsFromCache);
        Assert.Equal(cachedAt, sut.LastCacheTime);
    }

    [Fact]
    public async Task GetLatestRatesAsync_WhenNotCached_FetchesFromApi()
    {
        // Arrange
        var apiResponse = @"{
            ""rates"": {
                ""EUR"": 0.85,
                ""GBP"": 0.73,
                ""JPY"": 149.50
            }
        }";

        _cacheServiceMock
            .Setup(c => c.TryGetWithMetadata<List<ExchangeRate>>(
                It.IsAny<string>(),
                out It.Ref<List<ExchangeRate>?>.IsAny,
                out It.Ref<DateTime?>.IsAny))
            .Returns(false);

        var httpClient = CreateMockHttpClient(apiResponse);
        var sut = CreateService(httpClient);

        // Act
        var result = await sut.GetLatestRatesAsync("USD");

        // Assert
        Assert.Equal(3, result.Count);
        Assert.False(sut.IsFromCache);
        Assert.Contains(result, r => r.Currency == "EUR" && r.Rate == 0.85m);
        Assert.Contains(result, r => r.Currency == "GBP" && r.Rate == 0.73m);
        Assert.Contains(result, r => r.Currency == "JPY" && r.Rate == 149.50m);
    }

    [Fact]
    public async Task GetLatestRatesAsync_WhenApiReturnsEmpty_ReturnsEmptyList()
    {
        // Arrange
        var apiResponse = @"{ ""rates"": null }";

        _cacheServiceMock
            .Setup(c => c.TryGetWithMetadata<List<ExchangeRate>>(
                It.IsAny<string>(),
                out It.Ref<List<ExchangeRate>?>.IsAny,
                out It.Ref<DateTime?>.IsAny))
            .Returns(false);

        var httpClient = CreateMockHttpClient(apiResponse);
        var sut = CreateService(httpClient);

        // Act
        var result = await sut.GetLatestRatesAsync("USD");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLatestRatesAsync_CachesResult()
    {
        // Arrange
        var apiResponse = @"{ ""rates"": { ""EUR"": 0.85 } }";

        _cacheServiceMock
            .Setup(c => c.TryGetWithMetadata<List<ExchangeRate>>(
                It.IsAny<string>(),
                out It.Ref<List<ExchangeRate>?>.IsAny,
                out It.Ref<DateTime?>.IsAny))
            .Returns(false);

        var httpClient = CreateMockHttpClient(apiResponse);
        var sut = CreateService(httpClient);

        // Act
        await sut.GetLatestRatesAsync("USD");

        // Assert
        _cacheServiceMock.Verify(
            c => c.Set(
                "rates_USD",
                It.IsAny<List<ExchangeRate>>(),
                TimeSpan.FromMinutes(5)),
            Times.Once);
    }

    [Fact]
    public async Task GetAvailableCurrenciesAsync_WhenCached_ReturnsCachedData()
    {
        // Arrange
        var cachedCurrencies = new Dictionary<string, string>
        {
            { "USD", "US Dollar" },
            { "EUR", "Euro" }
        };

        _cacheServiceMock
            .Setup(c => c.TryGetWithMetadata<Dictionary<string, string>>(
                "currencies",
                out It.Ref<Dictionary<string, string>?>.IsAny,
                out It.Ref<DateTime?>.IsAny))
            .Callback(new TryGetWithMetadataCallback<Dictionary<string, string>>((string key, out Dictionary<string, string>? value, out DateTime? cached) =>
            {
                value = cachedCurrencies;
                cached = DateTime.UtcNow;
            }))
            .Returns(true);

        var httpClient = CreateMockHttpClient("{}");
        var sut = CreateService(httpClient);

        // Act
        var result = await sut.GetAvailableCurrenciesAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("US Dollar", result["USD"]);
    }

    [Fact]
    public async Task GetAvailableCurrenciesAsync_WhenNotCached_FetchesFromApi()
    {
        // Arrange
        var apiResponse = @"{
            ""USD"": ""United States Dollar"",
            ""EUR"": ""Euro"",
            ""GBP"": ""British Pound""
        }";

        _cacheServiceMock
            .Setup(c => c.TryGetWithMetadata<Dictionary<string, string>>(
                It.IsAny<string>(),
                out It.Ref<Dictionary<string, string>?>.IsAny,
                out It.Ref<DateTime?>.IsAny))
            .Returns(false);

        var httpClient = CreateMockHttpClient(apiResponse);
        var sut = CreateService(httpClient);

        // Act
        var result = await sut.GetAvailableCurrenciesAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("United States Dollar", result["USD"]);
    }

    [Fact]
    public async Task GetHistoricalRatesAsync_WhenNotCached_FetchesFromApi()
    {
        // Arrange
        var apiResponse = @"{
            ""rates"": {
                ""2024-01-01"": { ""EUR"": 0.84 },
                ""2024-01-02"": { ""EUR"": 0.85 },
                ""2024-01-03"": { ""EUR"": 0.86 }
            }
        }";

        _cacheServiceMock
            .Setup(c => c.TryGetWithMetadata<List<HistoricalRate>>(
                It.IsAny<string>(),
                out It.Ref<List<HistoricalRate>?>.IsAny,
                out It.Ref<DateTime?>.IsAny))
            .Returns(false);

        var httpClient = CreateMockHttpClient(apiResponse);
        var sut = CreateService(httpClient);

        // Act
        var result = await sut.GetHistoricalRatesAsync("USD", "EUR", 7);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(0.84m, result[0].Rate);
        Assert.Equal(0.86m, result[2].Rate);
    }

    [Fact]
    public async Task GetHistoricalRatesAsync_SortsResultsByDate()
    {
        // Arrange
        var apiResponse = @"{
            ""rates"": {
                ""2024-01-03"": { ""EUR"": 0.86 },
                ""2024-01-01"": { ""EUR"": 0.84 },
                ""2024-01-02"": { ""EUR"": 0.85 }
            }
        }";

        _cacheServiceMock
            .Setup(c => c.TryGetWithMetadata<List<HistoricalRate>>(
                It.IsAny<string>(),
                out It.Ref<List<HistoricalRate>?>.IsAny,
                out It.Ref<DateTime?>.IsAny))
            .Returns(false);

        var httpClient = CreateMockHttpClient(apiResponse);
        var sut = CreateService(httpClient);

        // Act
        var result = await sut.GetHistoricalRatesAsync("USD", "EUR", 7);

        // Assert
        Assert.Equal(new DateTime(2024, 1, 1), result[0].Date);
        Assert.Equal(new DateTime(2024, 1, 2), result[1].Date);
        Assert.Equal(new DateTime(2024, 1, 3), result[2].Date);
    }

    [Fact]
    public async Task ConvertAsync_WhenSameCurrency_ReturnsOriginalAmount()
    {
        // Arrange
        var httpClient = CreateMockHttpClient("{}");
        var sut = CreateService(httpClient);

        // Act
        var result = await sut.ConvertAsync("USD", "USD", 100m);

        // Assert
        Assert.Equal(100m, result);
    }

    [Fact]
    public async Task ConvertAsync_CalculatesCorrectConversion()
    {
        // Arrange
        var apiResponse = @"{ ""rates"": { ""EUR"": 0.85 } }";

        _cacheServiceMock
            .Setup(c => c.TryGetWithMetadata<List<ExchangeRate>>(
                It.IsAny<string>(),
                out It.Ref<List<ExchangeRate>?>.IsAny,
                out It.Ref<DateTime?>.IsAny))
            .Returns(false);

        var httpClient = CreateMockHttpClient(apiResponse);
        var sut = CreateService(httpClient);

        // Act
        var result = await sut.ConvertAsync("USD", "EUR", 100m);

        // Assert
        Assert.Equal(85m, result);
    }

    [Fact]
    public async Task ConvertAsync_WhenCurrencyNotFound_ReturnsZero()
    {
        // Arrange
        var apiResponse = @"{ ""rates"": { ""EUR"": 0.85 } }";

        _cacheServiceMock
            .Setup(c => c.TryGetWithMetadata<List<ExchangeRate>>(
                It.IsAny<string>(),
                out It.Ref<List<ExchangeRate>?>.IsAny,
                out It.Ref<DateTime?>.IsAny))
            .Returns(false);

        var httpClient = CreateMockHttpClient(apiResponse);
        var sut = CreateService(httpClient);

        // Act
        var result = await sut.ConvertAsync("USD", "GBP", 100m);

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public async Task GetLatestRatesAsync_WhenApiError_ThrowsException()
    {
        // Arrange
        _cacheServiceMock
            .Setup(c => c.TryGetWithMetadata<List<ExchangeRate>>(
                It.IsAny<string>(),
                out It.Ref<List<ExchangeRate>?>.IsAny,
                out It.Ref<DateTime?>.IsAny))
            .Returns(false);

        var httpClient = CreateMockHttpClient("Error", HttpStatusCode.InternalServerError);
        var sut = CreateService(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetLatestRatesAsync("USD"));
    }

    private delegate void TryGetWithMetadataCallback<T>(string key, out T? value, out DateTime? cachedAt);
}

internal class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string _response;
    private readonly HttpStatusCode _statusCode;

    public MockHttpMessageHandler(string response, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _response = response;
        _statusCode = statusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_response)
        };
        return Task.FromResult(response);
    }
}
