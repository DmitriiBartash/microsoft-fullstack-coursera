using CurrencyExchangeDashboard.Models;

namespace CurrencyExchangeDashboard.Tests.Models;

public class ExchangeRateTests
{
    [Fact]
    public void ExchangeRate_CreatesWithCorrectValues()
    {
        // Arrange & Act
        var now = DateTime.UtcNow;
        var rate = new ExchangeRate("EUR", 0.85m, now);

        // Assert
        Assert.Equal("EUR", rate.Currency);
        Assert.Equal(0.85m, rate.Rate);
        Assert.Equal(now, rate.UpdatedAt);
    }

    [Fact]
    public void ExchangeRate_ValueEquality_Works()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var rate1 = new ExchangeRate("EUR", 0.85m, now);
        var rate2 = new ExchangeRate("EUR", 0.85m, now);

        // Assert
        Assert.Equal(rate1, rate2);
        Assert.True(rate1 == rate2);
    }

    [Fact]
    public void ExchangeRate_Deconstruction_Works()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var rate = new ExchangeRate("EUR", 0.85m, now);

        // Act
        var (currency, rateValue, updatedAt) = rate;

        // Assert
        Assert.Equal("EUR", currency);
        Assert.Equal(0.85m, rateValue);
        Assert.Equal(now, updatedAt);
    }
}
