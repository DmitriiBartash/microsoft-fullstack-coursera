using CurrencyExchangeDashboard.Models;

namespace CurrencyExchangeDashboard.Tests.Models;

public class CachedDataTests
{
    [Fact]
    public void CachedData_CreatesWithCorrectValues()
    {
        // Arrange & Act
        var data = new List<string> { "a", "b" };
        var cachedAt = DateTime.UtcNow;
        var cached = new CachedData<List<string>>(data, cachedAt);

        // Assert
        Assert.Equal(data, cached.Data);
        Assert.Equal(cachedAt, cached.CachedAt);
    }

    [Fact]
    public void CachedData_WorksWithDifferentTypes()
    {
        // Arrange & Act
        var intCached = new CachedData<int>(42, DateTime.UtcNow);
        var stringCached = new CachedData<string>("test", DateTime.UtcNow);
        var listCached = new CachedData<List<int>>([1, 2, 3], DateTime.UtcNow);

        // Assert
        Assert.Equal(42, intCached.Data);
        Assert.Equal("test", stringCached.Data);
        Assert.Equal(3, listCached.Data.Count);
    }
}
