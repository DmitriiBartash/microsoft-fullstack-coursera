using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.SalesOrders.Queries;

/// <summary>
/// Query to get paginated list of sales orders.
/// </summary>
public sealed record GetSalesOrdersQuery : IRequest<Result<PaginatedList<SalesOrderDto>>>
{
    public string? Status { get; init; }
    public Guid? ClientId { get; init; }
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
