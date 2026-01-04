using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Command to ship a sales order (Confirmed -> Shipped).
/// This triggers Stock Out for all items.
/// </summary>
public sealed record ShipSalesOrderCommand(Guid OrderId) : IRequest<Result<Unit>>;
