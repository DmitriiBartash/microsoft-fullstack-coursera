namespace CurrencyExchangeDashboard.Models;

/// <summary>
/// Buy/Sell transaction record.
/// </summary>
public record Transaction(
    TransactionType Type,
    string Currency,
    decimal Amount,
    decimal Rate,
    decimal Total,
    DateTime Timestamp
);

public enum TransactionType
{
    Buy,
    Sell
}
