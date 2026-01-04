using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Suppliers.Commands;

/// <summary>
/// Handler for updating an existing supplier.
/// </summary>
public class UpdateSupplierHandler : IRequestHandler<UpdateSupplierCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public UpdateSupplierHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the command to update a supplier.
    /// </summary>
    public async Task<Result<Unit>> Handle(
        UpdateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<Unit>.Failure("Supplier.InvalidName", "Supplier name is required.");
        }

        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (supplier is null)
        {
            return Result<Unit>.Failure("Supplier.NotFound", $"Supplier with ID {request.Id} not found.");
        }

        var nameExists = await _context.Suppliers
            .AnyAsync(s => s.Id != request.Id && s.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (nameExists)
        {
            return Result<Unit>.Failure("Supplier.DuplicateName", $"Supplier with name '{request.Name}' already exists.");
        }

        supplier.Update(
            request.Name,
            request.ContactPerson,
            request.Email,
            request.Phone,
            request.Address);

        if (request.IsActive && !supplier.IsActive)
        {
            supplier.Activate();
        }
        else if (!request.IsActive && supplier.IsActive)
        {
            supplier.Deactivate();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
