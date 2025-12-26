using CurrencyExchangeDashboard.Models;

namespace CurrencyExchangeDashboard.Tests.Models;

public class WalletEntryTests
{
    [Fact]
    public void WalletEntry_CreatesWithCorrectValues()
    {
        // Arrange & Act
        var entry = new WalletEntry("USD", 1000m);

        // Assert
        Assert.Equal("USD", entry.Currency);
        Assert.Equal(1000m, entry.Amount);
    }

    [Fact]
    public void WalletEntry_ValueEquality_Works()
    {
        // Arrange
        var entry1 = new WalletEntry("EUR", 500m);
        var entry2 = new WalletEntry("EUR", 500m);

        // Assert
        Assert.Equal(entry1, entry2);
    }

    [Fact]
    public void WalletEntry_Deconstruction_Works()
    {
        // Arrange
        var entry = new WalletEntry("GBP", 250m);

        // Act
        var (currency, amount) = entry;

        // Assert
        Assert.Equal("GBP", currency);
        Assert.Equal(250m, amount);
    }

    [Fact]
    public void WalletEntry_WithExpression_CreatesModifiedCopy()
    {
        // Arrange
        var original = new WalletEntry("USD", 1000m);

        // Act
        var modified = original with { Amount = 1500m };

        // Assert
        Assert.Equal(1000m, original.Amount);
        Assert.Equal(1500m, modified.Amount);
        Assert.Equal("USD", modified.Currency);
    }

    [Fact]
    public void WalletEntry_HandlesZeroAmount()
    {
        // Arrange & Act
        var entry = new WalletEntry("USD", 0m);

        // Assert
        Assert.Equal(0m, entry.Amount);
    }

    [Fact]
    public void WalletEntry_HandlesLargeAmount()
    {
        // Arrange & Act
        var entry = new WalletEntry("USD", 999999999.99m);

        // Assert
        Assert.Equal(999999999.99m, entry.Amount);
    }
}
