using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Orders.Queries;

/// <summary>
/// Query to get purchase order by ID with items.
/// </summary>
public sealed record GetOrderByIdQuery(Guid Id) : IRequest<Result<PurchaseOrderDto>>;
