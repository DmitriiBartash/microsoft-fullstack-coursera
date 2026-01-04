using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Handler for marking a sales order as delivered.
/// </summary>
public sealed class DeliverSalesOrderHandler : IRequestHandler<DeliverSalesOrderCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public DeliverSalesOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(
        DeliverSalesOrderCommand request,
        CancellationToken ct)
    {
        var order = await _context.SalesOrders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            return Result<Unit>.Failure(
                "SALES_ORDER_NOT_FOUND",
                $"Sales order with ID {request.OrderId} was not found");
        }

        if (order.Status != SalesOrderStatus.Shipped)
        {
            return Result<Unit>.Failure(
                "INVALID_ORDER_STATUS",
                "Only shipped orders can be marked as delivered");
        }

        try
        {
            order.MarkDelivered();
        }
        catch (InvalidOperationException ex)
        {
            return Result<Unit>.Failure("INVALID_OPERATION", ex.Message);
        }

        await _context.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}
