using CurrencyExchangeDashboard.Services;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyExchangeDashboard.Tests.Services;

public class CacheServiceTests
{
    private readonly CacheService _sut;
    private readonly IMemoryCache _memoryCache;

    public CacheServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _sut = new CacheService(_memoryCache);
    }

    [Fact]
    public void Get_WhenKeyNotExists_ReturnsDefault()
    {
        // Act
        var result = _sut.Get<string>("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Get_WhenKeyExists_ReturnsValue()
    {
        // Arrange
        var key = "test_key";
        var value = "test_value";
        _sut.Set(key, value, TimeSpan.FromMinutes(5));

        // Act
        var result = _sut.Get<string>(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Set_StoresValueWithExpiration()
    {
        // Arrange
        var key = "expiring_key";
        var value = 42;

        // Act
        _sut.Set(key, value, TimeSpan.FromMinutes(10));

        // Assert
        var result = _sut.Get<int>(key);
        Assert.Equal(value, result);
    }

    [Fact]
    public void Remove_DeletesValueFromCache()
    {
        // Arrange
        var key = "to_remove";
        _sut.Set(key, "value", TimeSpan.FromMinutes(5));

        // Act
        _sut.Remove(key);

        // Assert
        var result = _sut.Get<string>(key);
        Assert.Null(result);
    }

    [Fact]
    public void TryGetWithMetadata_WhenKeyNotExists_ReturnsFalse()
    {
        // Act
        var result = _sut.TryGetWithMetadata<string>("nonexistent", out var value, out var cachedAt);

        // Assert
        Assert.False(result);
        Assert.Null(value);
        Assert.Null(cachedAt);
    }

    [Fact]
    public void TryGetWithMetadata_WhenKeyExists_ReturnsTrueWithMetadata()
    {
        // Arrange
        var key = "metadata_key";
        var value = "metadata_value";
        var beforeSet = DateTime.UtcNow;
        _sut.Set(key, value, TimeSpan.FromMinutes(5));
        var afterSet = DateTime.UtcNow;

        // Act
        var result = _sut.TryGetWithMetadata<string>(key, out var retrievedValue, out var cachedAt);

        // Assert
        Assert.True(result);
        Assert.Equal(value, retrievedValue);
        Assert.NotNull(cachedAt);
        Assert.True(cachedAt >= beforeSet && cachedAt <= afterSet);
    }

    [Fact]
    public void Set_OverwritesExistingValue()
    {
        // Arrange
        var key = "overwrite_key";
        _sut.Set(key, "original", TimeSpan.FromMinutes(5));

        // Act
        _sut.Set(key, "updated", TimeSpan.FromMinutes(5));

        // Assert
        var result = _sut.Get<string>(key);
        Assert.Equal("updated", result);
    }

    [Fact]
    public void Get_WithComplexType_ReturnsCorrectValue()
    {
        // Arrange
        var key = "complex_key";
        var value = new List<string> { "a", "b", "c" };
        _sut.Set(key, value, TimeSpan.FromMinutes(5));

        // Act
        var result = _sut.Get<List<string>>(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("a", result[0]);
    }

    [Fact]
    public void TryGetWithMetadata_WithComplexType_ReturnsCorrectData()
    {
        // Arrange
        var key = "complex_metadata";
        var value = new Dictionary<string, int> { { "USD", 1 }, { "EUR", 2 } };
        _sut.Set(key, value, TimeSpan.FromMinutes(5));

        // Act
        var result = _sut.TryGetWithMetadata<Dictionary<string, int>>(key, out var retrieved, out var cachedAt);

        // Assert
        Assert.True(result);
        Assert.NotNull(retrieved);
        Assert.Equal(2, retrieved.Count);
        Assert.NotNull(cachedAt);
    }
}
