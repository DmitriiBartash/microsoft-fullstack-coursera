namespace CurrencyExchangeDashboard.Models;

/// <summary>
/// Wrapper for cached data with metadata.
/// </summary>
public record CachedData<T>(
    T Data,
    DateTime CachedAt
);
