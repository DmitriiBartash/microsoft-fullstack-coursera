using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.SalesOrders.Queries;

/// <summary>
/// Query to get sales order by ID.
/// </summary>
public sealed record GetSalesOrderByIdQuery(Guid Id) : IRequest<Result<SalesOrderDto>>;
