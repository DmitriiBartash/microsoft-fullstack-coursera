namespace CurrencyExchangeDashboard.Models;

/// <summary>
/// Conversion operation record.
/// </summary>
public record ConversionRecord(
    string FromCurrency,
    string ToCurrency,
    decimal Amount,
    decimal Result,
    DateTime Timestamp
);
