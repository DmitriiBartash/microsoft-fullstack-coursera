using System.Text.Json.Serialization;

namespace InventoryHub.Client.Models;

/// <summary>
/// Dashboard statistics data transfer object
/// </summary>
public class DashboardStatsDto
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
    public int TotalCategories { get; set; }
    public int TotalSuppliers { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int PendingOrders { get; set; }
}

/// <summary>
/// Low stock product data transfer object
/// </summary>
public class LowStockProductDto
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public int StockNeeded => Math.Max(0, MinimumStock - CurrentStock);
}

/// <summary>
/// Recent inventory movement data transfer object
/// </summary>
public class RecentMovementDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductSKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MovementType Type { get; set; }

    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}
