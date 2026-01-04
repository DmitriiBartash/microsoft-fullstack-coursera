using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Command to create new purchase order.
/// </summary>
public sealed record CreateOrderCommand(
    Guid SupplierId,
    DateTime? ExpectedDeliveryDate,
    string? Notes,
    List<CreateOrderItemDto> Items) : IRequest<Result<Guid>>;

/// <summary>
/// Data transfer object for creating order item.
/// </summary>
public sealed record CreateOrderItemDto(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice);
