namespace CurrencyExchangeDashboard.Models;

/// <summary>
/// Current exchange rate for a currency.
/// </summary>
public record ExchangeRate(
    string Currency,
    decimal Rate,
    DateTime UpdatedAt
);
