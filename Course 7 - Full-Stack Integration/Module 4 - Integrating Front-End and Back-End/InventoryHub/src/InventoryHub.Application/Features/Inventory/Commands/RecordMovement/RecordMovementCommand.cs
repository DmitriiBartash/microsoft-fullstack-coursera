using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;

namespace InventoryHub.Application.Features.Inventory.Commands.RecordMovement;

/// <summary>
/// Command to record a stock movement for a product
/// </summary>
public sealed record RecordMovementCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Product identifier
    /// </summary>
    public required Guid ProductId { get; init; }

    /// <summary>
    /// Type of movement (In, Out, Adjust)
    /// </summary>
    public required MovementType Type { get; init; }

    /// <summary>
    /// Quantity to adjust (for Adjust type, this is the new stock level)
    /// </summary>
    public required int Quantity { get; init; }

    /// <summary>
    /// Optional reference (e.g., order number, receipt number)
    /// </summary>
    public string? Reference { get; init; }

    /// <summary>
    /// Optional notes about the movement
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// User who created this movement
    /// </summary>
    public string? CreatedBy { get; init; }
}
