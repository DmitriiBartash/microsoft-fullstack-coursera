using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Suppliers.Commands;

/// <summary>
/// Handler for deleting a supplier.
/// </summary>
public class DeleteSupplierHandler : IRequestHandler<DeleteSupplierCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public DeleteSupplierHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the command to delete a supplier.
    /// </summary>
    public async Task<Result<Unit>> Handle(
        DeleteSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (supplier is null)
        {
            return Result<Unit>.Failure("Supplier.NotFound", $"Supplier with ID {request.Id} not found.");
        }

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
