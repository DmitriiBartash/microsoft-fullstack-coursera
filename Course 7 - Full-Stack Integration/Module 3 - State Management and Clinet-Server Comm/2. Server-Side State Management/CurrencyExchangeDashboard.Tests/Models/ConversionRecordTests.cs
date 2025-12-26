using CurrencyExchangeDashboard.Models;

namespace CurrencyExchangeDashboard.Tests.Models;

public class ConversionRecordTests
{
    [Fact]
    public void ConversionRecord_CreatesWithCorrectValues()
    {
        // Arrange & Act
        var timestamp = DateTime.UtcNow;
        var record = new ConversionRecord("USD", "EUR", 100m, 85m, timestamp);

        // Assert
        Assert.Equal("USD", record.FromCurrency);
        Assert.Equal("EUR", record.ToCurrency);
        Assert.Equal(100m, record.Amount);
        Assert.Equal(85m, record.Result);
        Assert.Equal(timestamp, record.Timestamp);
    }

    [Fact]
    public void ConversionRecord_ValueEquality_Works()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var record1 = new ConversionRecord("USD", "EUR", 100m, 85m, timestamp);
        var record2 = new ConversionRecord("USD", "EUR", 100m, 85m, timestamp);

        // Assert
        Assert.Equal(record1, record2);
    }

    [Fact]
    public void ConversionRecord_Deconstruction_Works()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var record = new ConversionRecord("USD", "EUR", 100m, 85m, timestamp);

        // Act
        var (from, to, amount, result, time) = record;

        // Assert
        Assert.Equal("USD", from);
        Assert.Equal("EUR", to);
        Assert.Equal(100m, amount);
        Assert.Equal(85m, result);
        Assert.Equal(timestamp, time);
    }
}
