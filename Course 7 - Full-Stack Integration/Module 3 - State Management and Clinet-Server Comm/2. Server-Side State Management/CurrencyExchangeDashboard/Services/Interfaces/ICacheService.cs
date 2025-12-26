namespace CurrencyExchangeDashboard.Services.Interfaces;

public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expiration);
    void Remove(string key);
    bool TryGetWithMetadata<T>(string key, out T? value, out DateTime? cachedAt);
}
