using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using InventoryHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Suppliers.Commands;

/// <summary>
/// Handler for creating a new supplier.
/// </summary>
public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateSupplierHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the command to create a new supplier.
    /// </summary>
    public async Task<Result<Guid>> Handle(
        CreateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<Guid>.Failure("Supplier.InvalidName", "Supplier name is required.");
        }

        var nameExists = await _context.Suppliers
            .AnyAsync(s => s.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (nameExists)
        {
            return Result<Guid>.Failure("Supplier.DuplicateName", $"Supplier with name '{request.Name}' already exists.");
        }

        var supplier = Supplier.Create(
            request.Name,
            request.ContactPerson,
            request.Email,
            request.Phone,
            request.Address);

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(supplier.Id);
    }
}
