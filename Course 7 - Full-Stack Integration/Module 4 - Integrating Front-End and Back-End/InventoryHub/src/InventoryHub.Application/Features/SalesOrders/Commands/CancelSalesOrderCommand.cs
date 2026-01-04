using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Command to cancel a sales order.
/// Cannot cancel shipped or delivered orders.
/// </summary>
public sealed record CancelSalesOrderCommand(Guid OrderId) : IRequest<Result<Unit>>;
