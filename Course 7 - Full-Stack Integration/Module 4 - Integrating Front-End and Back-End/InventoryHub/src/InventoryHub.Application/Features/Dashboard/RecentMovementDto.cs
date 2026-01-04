using InventoryHub.Domain.Enums;

namespace InventoryHub.Application.Features.Dashboard;

/// <summary>
/// Data transfer object for recent stock movement information
/// </summary>
public sealed class RecentMovementDto
{
    /// <summary>
    /// Movement identifier
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Product identifier
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Product SKU
    /// </summary>
    public string ProductSKU { get; init; } = null!;

    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; init; } = null!;

    /// <summary>
    /// Type of stock movement (In, Out, Adjust)
    /// </summary>
    public MovementType Type { get; init; }

    /// <summary>
    /// Quantity moved
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Stock level before the movement
    /// </summary>
    public int PreviousStock { get; init; }

    /// <summary>
    /// Stock level after the movement
    /// </summary>
    public int NewStock { get; init; }

    /// <summary>
    /// Reference number or identifier for the movement
    /// </summary>
    public string? Reference { get; init; }

    /// <summary>
    /// Additional notes about the movement
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// User who created the movement
    /// </summary>
    public string? CreatedBy { get; init; }

    /// <summary>
    /// Timestamp when the movement was created
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
