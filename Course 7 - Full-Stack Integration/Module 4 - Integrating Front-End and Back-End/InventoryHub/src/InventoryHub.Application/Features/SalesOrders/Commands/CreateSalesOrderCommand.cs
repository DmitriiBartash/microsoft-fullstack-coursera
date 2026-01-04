using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Command to create new sales order.
/// </summary>
public sealed record CreateSalesOrderCommand(
    Guid ClientId,
    DateTime? RequiredDate,
    string? ShippingAddress,
    string? Notes,
    List<CreateSalesOrderItemDto> Items) : IRequest<Result<Guid>>;

/// <summary>
/// Data transfer object for creating sales order item.
/// </summary>
public sealed record CreateSalesOrderItemDto(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice);
