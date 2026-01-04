using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Command to receive purchase order items and update stock.
/// </summary>
public sealed record ReceiveOrderCommand(
    Guid OrderId,
    List<ReceiveOrderItemDto> Items) : IRequest<Result<Unit>>;

/// <summary>
/// Data transfer object for receiving order item.
/// </summary>
public sealed record ReceiveOrderItemDto(
    Guid OrderItemId,
    int ReceivedQuantity);
