using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Suppliers.Queries;

/// <summary>
/// Handler for retrieving suppliers with optional filtering and product count.
/// </summary>
public class GetSuppliersHandler : IRequestHandler<GetSuppliersQuery, Result<List<SupplierDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetSuppliersHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the query to retrieve suppliers.
    /// </summary>
    public async Task<Result<List<SupplierDto>>> Handle(
        GetSuppliersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Suppliers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(searchLower) ||
                (s.ContactPerson != null && s.ContactPerson.ToLower().Contains(searchLower)) ||
                (s.Email != null && s.Email.ToLower().Contains(searchLower)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == request.IsActive.Value);
        }

        var suppliers = await query
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
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

        return Result<List<SupplierDto>>.Success(suppliers);
    }
}
