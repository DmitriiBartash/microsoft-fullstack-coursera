using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Handler for changing sales order status directly with inventory updates.
/// </summary>
public sealed class ChangeSalesOrderStatusHandler : IRequestHandler<ChangeSalesOrderStatusCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public ChangeSalesOrderStatusHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(
        ChangeSalesOrderStatusCommand request,
        CancellationToken ct)
    {
        var order = await _context.SalesOrders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            return Result<Unit>.Failure(
                "ORDER_NOT_FOUND",
                $"Sales order with ID {request.OrderId} was not found");
        }

        var previousStatus = order.Status;
        var wasShippedOrDelivered = previousStatus is SalesOrderStatus.Shipped or SalesOrderStatus.Delivered;

        if (request.NewStatus == SalesOrderStatus.Shipped && !wasShippedOrDelivered)
        {
            foreach (var item in order.Items)
            {
                var stockMovement = item.Product.AdjustStock(
                    -item.Quantity,
                    MovementType.Out,
                    reference: order.OrderNumber,
                    notes: $"Shipped in sales order {order.OrderNumber}",
                    createdBy: "System");

                _context.StockMovements.Add(stockMovement);
            }
        }

        if (request.NewStatus == SalesOrderStatus.Delivered && !wasShippedOrDelivered)
        {
            foreach (var item in order.Items)
            {
                var stockMovement = item.Product.AdjustStock(
                    -item.Quantity,
                    MovementType.Out,
                    reference: order.OrderNumber,
                    notes: $"Delivered in sales order {order.OrderNumber}",
                    createdBy: "System");

                _context.StockMovements.Add(stockMovement);
            }
        }

        if (request.NewStatus == SalesOrderStatus.Cancelled && wasShippedOrDelivered)
        {
            foreach (var item in order.Items)
            {
                var stockMovement = item.Product.AdjustStock(
                    item.Quantity,
                    MovementType.In,
                    reference: order.OrderNumber,
                    notes: $"Cancelled sales order {order.OrderNumber} (returned to inventory)",
                    createdBy: "System");

                _context.StockMovements.Add(stockMovement);
            }
        }

        order.SetStatus(request.NewStatus);
        await _context.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}
