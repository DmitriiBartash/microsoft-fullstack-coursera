using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Handler for cancelling a sales order.
/// </summary>
public sealed class CancelSalesOrderHandler : IRequestHandler<CancelSalesOrderCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public CancelSalesOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(
        CancelSalesOrderCommand request,
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

        if (order.Status == SalesOrderStatus.Shipped)
        {
            return Result<Unit>.Failure(
                "CANNOT_CANCEL_SHIPPED",
                "Cannot cancel a shipped order (stock already deducted)");
        }

        if (order.Status == SalesOrderStatus.Delivered)
        {
            return Result<Unit>.Failure(
                "CANNOT_CANCEL_DELIVERED",
                "Cannot cancel a delivered order");
        }

        if (order.Status == SalesOrderStatus.Cancelled)
        {
            return Result<Unit>.Failure(
                "ALREADY_CANCELLED",
                "Order is already cancelled");
        }

        try
        {
            order.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return Result<Unit>.Failure("INVALID_OPERATION", ex.Message);
        }

        await _context.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}
