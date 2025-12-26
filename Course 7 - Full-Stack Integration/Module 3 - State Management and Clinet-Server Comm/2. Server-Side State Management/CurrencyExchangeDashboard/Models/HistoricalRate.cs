namespace CurrencyExchangeDashboard.Models;

/// <summary>
/// Historical rate point for chart.
/// </summary>
public record HistoricalRate(
    DateTime Date,
    decimal Rate
);
