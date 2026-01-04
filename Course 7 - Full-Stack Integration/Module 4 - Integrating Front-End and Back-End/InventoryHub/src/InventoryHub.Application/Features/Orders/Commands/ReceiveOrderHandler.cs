using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Handler for receiving purchase order items and updating stock.
/// </summary>
public sealed class ReceiveOrderHandler : IRequestHandler<ReceiveOrderCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public ReceiveOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(
        ReceiveOrderCommand request,
        CancellationToken ct)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            return Result<Unit>.Failure(
                "ORDER_NOT_FOUND",
                $"Purchase order with ID {request.OrderId} was not found");
        }

        if (order.Status != OrderStatus.Shipped)
        {
            return Result<Unit>.Failure(
                "INVALID_ORDER_STATUS",
                "Only shipped orders can be received");
        }

        var itemsToReceive = request.Items.Count > 0
            ? request.Items
            : order.Items
                .Where(i => !i.IsFullyReceived)
                .Select(i => new ReceiveOrderItemDto(i.Id, i.Quantity - i.ReceivedQuantity))
                .ToList();

        if (itemsToReceive.Count == 0)
        {
            return Result<Unit>.Failure(
                "NO_ITEMS_TO_RECEIVE",
                "All items have already been received");
        }

        var orderItemIds = itemsToReceive.Select(i => i.OrderItemId).ToList();
        var orderItems = order.Items
            .Where(i => orderItemIds.Contains(i.Id))
            .ToDictionary(i => i.Id);

        var missingItems = orderItemIds.Except(orderItems.Keys).ToList();
        if (missingItems.Any())
        {
            return Result<Unit>.Failure(
                "ITEMS_NOT_FOUND",
                $"Some order items were not found: {string.Join(", ", missingItems)}");
        }

        foreach (var receiveItem in itemsToReceive)
        {
            if (receiveItem.ReceivedQuantity <= 0)
            {
                return Result<Unit>.Failure(
                    "INVALID_QUANTITY",
                    "Received quantity must be greater than zero");
            }

            var orderItem = orderItems[receiveItem.OrderItemId];
            var remainingToReceive = orderItem.Quantity - orderItem.ReceivedQuantity;

            if (receiveItem.ReceivedQuantity > remainingToReceive)
            {
                return Result<Unit>.Failure(
                    "QUANTITY_EXCEEDS_ORDERED",
                    $"Cannot receive {receiveItem.ReceivedQuantity} units of {orderItem.Product.Name}. Only {remainingToReceive} units remaining to receive");
            }

            orderItem.RecordReceived(receiveItem.ReceivedQuantity);

            var stockMovement = orderItem.Product.AdjustStock(
                receiveItem.ReceivedQuantity,
                MovementType.In,
                reference: order.OrderNumber,
                notes: $"Received from purchase order {order.OrderNumber}",
                createdBy: "System");

            _context.StockMovements.Add(stockMovement);
        }

        if (order.Items.All(i => i.IsFullyReceived))
        {
            try
            {
                order.MarkReceived();
            }
            catch (InvalidOperationException ex)
            {
                return Result<Unit>.Failure(
                    "INVALID_OPERATION",
                    ex.Message);
            }
        }

        await _context.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}
