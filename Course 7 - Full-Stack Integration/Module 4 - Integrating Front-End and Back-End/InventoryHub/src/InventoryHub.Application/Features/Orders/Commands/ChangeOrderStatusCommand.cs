using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Command to change order status directly.
/// </summary>
public sealed record ChangeOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus) : IRequest<Result<Unit>>;
