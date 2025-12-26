using CurrencyExchangeDashboard.Models;

namespace CurrencyExchangeDashboard.Tests.Models;

public class UserPreferencesTests
{
    [Fact]
    public void UserPreferences_CreatesWithCorrectValues()
    {
        // Arrange & Act
        var favorites = new List<string> { "USD", "EUR", "GBP" };
        var prefs = new UserPreferences("USD", favorites);

        // Assert
        Assert.Equal("USD", prefs.BaseCurrency);
        Assert.Equal(3, prefs.FavoriteCurrencies.Count);
    }

    [Fact]
    public void UserPreferences_Default_HasExpectedValues()
    {
        // Act
        var defaultPrefs = UserPreferences.Default;

        // Assert
        Assert.Equal("USD", defaultPrefs.BaseCurrency);
        Assert.Contains("USD", defaultPrefs.FavoriteCurrencies);
        Assert.Contains("EUR", defaultPrefs.FavoriteCurrencies);
        Assert.Contains("GBP", defaultPrefs.FavoriteCurrencies);
        Assert.Contains("JPY", defaultPrefs.FavoriteCurrencies);
        Assert.Contains("CNY", defaultPrefs.FavoriteCurrencies);
        Assert.Contains("CHF", defaultPrefs.FavoriteCurrencies);
    }

    [Fact]
    public void UserPreferences_Default_ContainsAllExpectedCurrencies()
    {
        // Act
        var defaultPrefs = UserPreferences.Default;

        // Assert
        Assert.Equal(10, defaultPrefs.FavoriteCurrencies.Count);
    }

    [Fact]
    public void UserPreferences_WithExpression_CreatesModifiedCopy()
    {
        // Arrange
        var original = new UserPreferences("USD", ["USD", "EUR"]);

        // Act
        var modified = original with { BaseCurrency = "EUR" };

        // Assert
        Assert.Equal("USD", original.BaseCurrency);
        Assert.Equal("EUR", modified.BaseCurrency);
        Assert.Equal(original.FavoriteCurrencies, modified.FavoriteCurrencies);
    }

    [Fact]
    public void UserPreferences_ValueEquality_Works()
    {
        // Arrange
        var favorites = new List<string> { "USD", "EUR" };
        var prefs1 = new UserPreferences("USD", favorites);
        var prefs2 = new UserPreferences("USD", favorites);

        // Assert
        Assert.Equal(prefs1, prefs2);
    }
}
