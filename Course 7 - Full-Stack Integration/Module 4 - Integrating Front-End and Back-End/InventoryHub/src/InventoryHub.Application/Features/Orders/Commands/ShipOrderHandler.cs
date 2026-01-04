using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Orders.Commands;

/// <summary>
/// Handler for marking purchase order as shipped.
/// </summary>
public sealed class ShipOrderHandler : IRequestHandler<ShipOrderCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public ShipOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(
        ShipOrderCommand request,
        CancellationToken ct)
    {
        var order = await _context.PurchaseOrders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            return Result<Unit>.Failure(
                "ORDER_NOT_FOUND",
                $"Purchase order with ID {request.OrderId} was not found");
        }

        try
        {
            order.MarkShipped();
            await _context.SaveChangesAsync(ct);

            return Result<Unit>.Success(Unit.Value);
        }
        catch (InvalidOperationException ex)
        {
            return Result<Unit>.Failure(
                "INVALID_OPERATION",
                ex.Message);
        }
    }
}
