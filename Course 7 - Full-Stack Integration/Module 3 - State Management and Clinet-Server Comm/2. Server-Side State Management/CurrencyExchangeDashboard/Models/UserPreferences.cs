namespace CurrencyExchangeDashboard.Models;

/// <summary>
/// User preferences stored in session.
/// </summary>
public record UserPreferences(
    string BaseCurrency,
    List<string> FavoriteCurrencies
)
{
    public static UserPreferences Default => new(
        "USD",
        ["USD", "EUR", "GBP", "JPY", "CNY", "CHF", "AUD", "CAD", "PLN", "RON"]
    );
}
