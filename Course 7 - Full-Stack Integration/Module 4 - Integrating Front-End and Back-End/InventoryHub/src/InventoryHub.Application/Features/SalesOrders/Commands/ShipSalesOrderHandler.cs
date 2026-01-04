using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Handler for shipping a sales order.
/// Creates Stock Out movements for all items.
/// </summary>
public sealed class ShipSalesOrderHandler : IRequestHandler<ShipSalesOrderCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public ShipSalesOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(
        ShipSalesOrderCommand request,
        CancellationToken ct)
    {
        var order = await _context.SalesOrders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            return Result<Unit>.Failure(
                "SALES_ORDER_NOT_FOUND",
                $"Sales order with ID {request.OrderId} was not found");
        }

        if (order.Status != SalesOrderStatus.Confirmed)
        {
            return Result<Unit>.Failure(
                "INVALID_ORDER_STATUS",
                "Only confirmed orders can be shipped");
        }

        var insufficientStock = order.Items
            .Where(i => i.Product.CurrentStock < i.Quantity)
            .Select(i => $"{i.Product.Name} (need {i.Quantity}, have {i.Product.CurrentStock})")
            .ToList();

        if (insufficientStock.Any())
        {
            return Result<Unit>.Failure(
                "INSUFFICIENT_STOCK",
                $"Insufficient stock for: {string.Join(", ", insufficientStock)}");
        }

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

        try
        {
            order.Ship();
        }
        catch (InvalidOperationException ex)
        {
            return Result<Unit>.Failure("INVALID_OPERATION", ex.Message);
        }

        await _context.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}
