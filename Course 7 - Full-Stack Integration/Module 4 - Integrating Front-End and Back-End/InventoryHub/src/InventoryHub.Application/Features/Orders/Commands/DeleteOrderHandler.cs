using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Handler for deleting draft purchase order.
/// </summary>
public sealed class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public DeleteOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(
        DeleteOrderCommand request,
        CancellationToken ct)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            return Result<Unit>.Failure(
                "ORDER_NOT_FOUND",
                $"Purchase order with ID {request.OrderId} was not found");
        }

        if (order.Status != OrderStatus.Draft)
        {
            return Result<Unit>.Failure(
                "INVALID_OPERATION",
                "Only draft orders can be deleted");
        }

        _context.PurchaseOrders.Remove(order);
        await _context.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}
