using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Suppliers.Queries;

/// <summary>
/// Handler for retrieving a single supplier by ID.
/// </summary>
public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSupplierByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the query to retrieve a supplier by ID.
    /// </summary>
    public async Task<Result<SupplierDto>> Handle(
        GetSupplierByIdQuery request,
        CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .Where(s => s.Id == request.Id)
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address,
                IsActive = s.IsActive,
                ProductCount = s.Products.Count,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (supplier is null)
        {
            return Result<SupplierDto>.Failure("Supplier.NotFound", $"Supplier with ID {request.Id} not found.");
        }

        return Result<SupplierDto>.Success(supplier);
    }
}
