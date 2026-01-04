namespace InventoryHub.Application.Features.Dashboard;

/// <summary>
/// Data transfer object for low stock product information
/// </summary>
public sealed class LowStockProductDto
{
    /// <summary>
    /// Product identifier
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Product SKU (Stock Keeping Unit)
    /// </summary>
    public string SKU { get; init; } = null!;

    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Category name
    /// </summary>
    public string CategoryName { get; init; } = null!;

    /// <summary>
    /// Current stock level
    /// </summary>
    public int CurrentStock { get; init; }

    /// <summary>
    /// Minimum stock threshold
    /// </summary>
    public int MinimumStock { get; init; }

    /// <summary>
    /// Stock deficit (MinimumStock - CurrentStock)
    /// </summary>
    public int StockDeficit { get; init; }

    /// <summary>
    /// Product price
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    /// Supplier name
    /// </summary>
    public string? SupplierName { get; init; }

    /// <summary>
    /// Indicates if product is completely out of stock
    /// </summary>
    public bool IsOutOfStock { get; init; }
}
