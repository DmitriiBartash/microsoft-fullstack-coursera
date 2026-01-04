namespace InventoryHub.Application.Features.Dashboard;

/// <summary>
/// Data transfer object for dashboard statistics
/// </summary>
public sealed class DashboardStatsDto
{
    /// <summary>
    /// Total number of products in the system
    /// </summary>
    public int TotalProducts { get; init; }

    /// <summary>
    /// Number of active products
    /// </summary>
    public int ActiveProducts { get; init; }

    /// <summary>
    /// Number of products with low stock (CurrentStock <= MinimumStock)
    /// </summary>
    public int LowStockCount { get; init; }

    /// <summary>
    /// Number of products that are out of stock (CurrentStock = 0)
    /// </summary>
    public int OutOfStockCount { get; init; }

    /// <summary>
    /// Total number of categories
    /// </summary>
    public int TotalCategories { get; init; }

    /// <summary>
    /// Total number of suppliers
    /// </summary>
    public int TotalSuppliers { get; init; }

    /// <summary>
    /// Number of purchase orders in pending status (Draft or Submitted)
    /// </summary>
    public int PendingOrders { get; init; }

    /// <summary>
    /// Total value of all inventory (sum of CurrentStock * Price for all products)
    /// </summary>
    public decimal TotalInventoryValue { get; init; }
}
