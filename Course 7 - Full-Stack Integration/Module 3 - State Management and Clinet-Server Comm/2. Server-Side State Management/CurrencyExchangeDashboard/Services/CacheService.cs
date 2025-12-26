using CurrencyExchangeDashboard.Models;
using CurrencyExchangeDashboard.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyExchangeDashboard.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out CachedData<T>? cached) && cached is not null)
        {
            return cached.Data;
        }
        return default;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        var cached = new CachedData<T>(value, DateTime.UtcNow);
        _cache.Set(key, cached, expiration);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }

    public bool TryGetWithMetadata<T>(string key, out T? value, out DateTime? cachedAt)
    {
        if (_cache.TryGetValue(key, out CachedData<T>? cached) && cached is not null)
        {
            value = cached.Data;
            cachedAt = cached.CachedAt;
            return true;
        }

        value = default;
        cachedAt = null;
        return false;
    }
}
