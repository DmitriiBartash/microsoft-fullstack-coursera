using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Command to mark a sales order as delivered (Shipped -> Delivered).
/// </summary>
public sealed record DeliverSalesOrderCommand(Guid OrderId) : IRequest<Result<Unit>>;
