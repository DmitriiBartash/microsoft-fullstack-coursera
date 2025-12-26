namespace CurrencyExchangeDashboard.Models;

/// <summary>
/// User wallet entry.
/// </summary>
public record WalletEntry(
    string Currency,
    decimal Amount
);
