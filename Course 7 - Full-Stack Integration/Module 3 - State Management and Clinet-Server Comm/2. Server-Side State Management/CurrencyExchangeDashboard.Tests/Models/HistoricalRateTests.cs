using CurrencyExchangeDashboard.Models;

namespace CurrencyExchangeDashboard.Tests.Models;

public class HistoricalRateTests
{
    [Fact]
    public void HistoricalRate_CreatesWithCorrectValues()
    {
        // Arrange & Act
        var date = new DateTime(2024, 1, 15);
        var rate = new HistoricalRate(date, 0.84m);

        // Assert
        Assert.Equal(date, rate.Date);
        Assert.Equal(0.84m, rate.Rate);
    }

    [Fact]
    public void HistoricalRate_ValueEquality_Works()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var rate1 = new HistoricalRate(date, 0.84m);
        var rate2 = new HistoricalRate(date, 0.84m);

        // Assert
        Assert.Equal(rate1, rate2);
    }
}
