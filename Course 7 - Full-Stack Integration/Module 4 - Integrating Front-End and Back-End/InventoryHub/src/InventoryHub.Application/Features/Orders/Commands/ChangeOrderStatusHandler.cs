using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Handler for changing order status directly with inventory updates.
/// </summary>
public sealed class ChangeOrderStatusHandler : IRequestHandler<ChangeOrderStatusCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public ChangeOrderStatusHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(
        ChangeOrderStatusCommand request,
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

        var previousStatus = order.Status;

        if (request.NewStatus == OrderStatus.Received && previousStatus != OrderStatus.Received)
        {
            foreach (var item in order.Items)
            {
                var stockMovement = item.Product.AdjustStock(
                    item.Quantity,
                    MovementType.In,
                    reference: order.OrderNumber,
                    notes: $"Received from purchase order {order.OrderNumber}",
                    createdBy: "System");

                _context.StockMovements.Add(stockMovement);
            }
        }

        if (request.NewStatus == OrderStatus.Cancelled && previousStatus == OrderStatus.Received)
        {
            foreach (var item in order.Items)
            {
                var stockMovement = item.Product.AdjustStock(
                    -item.Quantity,
                    MovementType.Out,
                    reference: order.OrderNumber,
                    notes: $"Cancelled purchase order {order.OrderNumber} (rollback)",
                    createdBy: "System");

                _context.StockMovements.Add(stockMovement);
            }
        }

        order.SetStatus(request.NewStatus);
        await _context.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}
