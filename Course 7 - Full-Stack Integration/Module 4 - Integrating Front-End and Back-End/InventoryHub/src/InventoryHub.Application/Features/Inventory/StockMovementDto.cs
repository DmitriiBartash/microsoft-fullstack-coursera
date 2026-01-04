namespace InventoryHub.Application.Features.Inventory;

/// <summary>
/// Stock movement data transfer object
/// </summary>
public sealed class StockMovementDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = null!;
    public string ProductSKU { get; init; } = null!;
    public string Type { get; init; } = null!;
    public int Quantity { get; init; }
    public int PreviousStock { get; init; }
    public int NewStock { get; init; }
    public string? Reference { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
}
