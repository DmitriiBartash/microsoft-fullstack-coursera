using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Inventory.Commands.RecordMovement;

/// <summary>
/// Handler for recording stock movements
/// </summary>
public sealed class RecordMovementHandler : IRequestHandler<RecordMovementCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public RecordMovementHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the stock movement recording request
    /// </summary>
    public async Task<Result<Guid>> Handle(RecordMovementCommand request, CancellationToken ct)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product is null)
        {
            return Result<Guid>.Failure("Movement.ProductNotFound", $"Product with ID '{request.ProductId}' does not exist.");
        }

        if (request.Quantity < 0)
        {
            return Result<Guid>.Failure("Movement.InvalidQuantity", "Quantity must be greater than or equal to zero.");
        }

        if (request.Type == MovementType.Out && product.CurrentStock < request.Quantity)
        {
            return Result<Guid>.Failure(
                "Movement.InsufficientStock",
                $"Cannot remove {request.Quantity} items. Current stock is {product.CurrentStock}.");
        }

        var movement = product.AdjustStock(
            request.Quantity,
            request.Type,
            request.Reference,
            request.Notes,
            request.CreatedBy);

        _context.StockMovements.Add(movement);
        await _context.SaveChangesAsync(ct);

        return Result<Guid>.Success(movement.Id);
    }
}
