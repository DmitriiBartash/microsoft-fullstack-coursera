using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;

namespace InventoryHub.Application.Features.Inventory.Queries.GetMovements;

/// <summary>
/// Query to retrieve paginated list of stock movements with optional filters
/// </summary>
public sealed record GetMovementsQuery : IRequest<Result<PaginatedList<StockMovementDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid? ProductId { get; init; }
    public MovementType? Type { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}
