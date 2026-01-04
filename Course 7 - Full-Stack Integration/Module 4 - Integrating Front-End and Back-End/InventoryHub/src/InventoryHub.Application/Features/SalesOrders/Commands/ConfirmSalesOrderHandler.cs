using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Handler for confirming a sales order.
/// </summary>
public sealed class ConfirmSalesOrderHandler : IRequestHandler<ConfirmSalesOrderCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public ConfirmSalesOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(
        ConfirmSalesOrderCommand request,
        CancellationToken ct)
    {
        var order = await _context.SalesOrders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            return Result<Unit>.Failure(
                "SALES_ORDER_NOT_FOUND",
                $"Sales order with ID {request.OrderId} was not found");
        }

        if (order.Status != SalesOrderStatus.Draft)
        {
            return Result<Unit>.Failure(
                "INVALID_ORDER_STATUS",
                "Only draft orders can be confirmed");
        }

        if (!order.Items.Any())
        {
            return Result<Unit>.Failure(
                "EMPTY_ORDER",
                "Cannot confirm an empty order");
        }

        try
        {
            order.Confirm();
        }
        catch (InvalidOperationException ex)
        {
            return Result<Unit>.Failure("INVALID_OPERATION", ex.Message);
        }

        await _context.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}
