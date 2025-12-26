using CurrencyExchangeDashboard.Models;

namespace CurrencyExchangeDashboard.Tests.Models;

public class TransactionTests
{
    [Fact]
    public void Transaction_CreatesWithBuyType()
    {
        // Arrange & Act
        var timestamp = DateTime.UtcNow;
        var transaction = new Transaction(TransactionType.Buy, "EUR", 100m, 0.85m, 85m, timestamp);

        // Assert
        Assert.Equal(TransactionType.Buy, transaction.Type);
        Assert.Equal("EUR", transaction.Currency);
        Assert.Equal(100m, transaction.Amount);
        Assert.Equal(0.85m, transaction.Rate);
        Assert.Equal(85m, transaction.Total);
    }

    [Fact]
    public void Transaction_CreatesWithSellType()
    {
        // Arrange & Act
        var timestamp = DateTime.UtcNow;
        var transaction = new Transaction(TransactionType.Sell, "GBP", 50m, 1.25m, 62.5m, timestamp);

        // Assert
        Assert.Equal(TransactionType.Sell, transaction.Type);
        Assert.Equal("GBP", transaction.Currency);
    }

    [Fact]
    public void TransactionType_HasCorrectValues()
    {
        // Assert
        Assert.Equal(0, (int)TransactionType.Buy);
        Assert.Equal(1, (int)TransactionType.Sell);
    }

    [Fact]
    public void Transaction_ValueEquality_Works()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var t1 = new Transaction(TransactionType.Buy, "EUR", 100m, 0.85m, 85m, timestamp);
        var t2 = new Transaction(TransactionType.Buy, "EUR", 100m, 0.85m, 85m, timestamp);

        // Assert
        Assert.Equal(t1, t2);
    }
}
