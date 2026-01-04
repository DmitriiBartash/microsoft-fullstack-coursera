using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Orders.Queries;

/// <summary>
/// Query to get paginated list of purchase orders with filters.
/// </summary>
public sealed record GetOrdersQuery(
    int Page = 1,
    int PageSize = 10,
    string? Status = null,
    Guid? SupplierId = null,
    string? Search = null) : IRequest<Result<PaginatedList<PurchaseOrderDto>>>;
