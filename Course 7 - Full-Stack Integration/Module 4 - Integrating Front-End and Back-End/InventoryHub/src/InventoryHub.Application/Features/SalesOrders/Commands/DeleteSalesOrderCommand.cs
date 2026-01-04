using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Command to delete a sales order.
/// Only draft orders can be deleted.
/// </summary>
public sealed record DeleteSalesOrderCommand(Guid OrderId) : IRequest<Result<Unit>>;
