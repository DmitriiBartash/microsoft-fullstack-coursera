using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;

namespace InventoryHub.Application.Features.Dashboard.Queries;

/// <summary>
/// Query to retrieve recent stock movements
/// </summary>
public sealed record GetRecentMovementsQuery : IRequest<Result<List<RecentMovementDto>>>
{
    /// <summary>
    /// Maximum number of movements to return. Default is 20.
    /// </summary>
    public int Limit { get; init; } = 20;

    /// <summary>
    /// Filter by movement type
    /// </summary>
    public MovementType? Type { get; init; }

    /// <summary>
    /// Filter by product identifier
    /// </summary>
    public Guid? ProductId { get; init; }
}
