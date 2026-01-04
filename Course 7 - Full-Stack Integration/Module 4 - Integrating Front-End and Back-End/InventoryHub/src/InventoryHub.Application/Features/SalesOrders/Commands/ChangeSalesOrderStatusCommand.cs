using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Command to change sales order status directly (admin override).
/// </summary>
public sealed record ChangeSalesOrderStatusCommand(
    Guid OrderId,
    SalesOrderStatus NewStatus) : IRequest<Result<Unit>>;
