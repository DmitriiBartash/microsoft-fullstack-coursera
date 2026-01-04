using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.SalesOrders.Commands;

/// <summary>
/// Handler for deleting a sales order.
/// </summary>
public sealed class DeleteSalesOrderHandler : IRequestHandler<DeleteSalesOrderCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public DeleteSalesOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(
        DeleteSalesOrderCommand request,
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
                "CANNOT_DELETE",
                "Only draft orders can be deleted. Use cancel for confirmed orders.");
        }

        _context.SalesOrders.Remove(order);
        await _context.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}
