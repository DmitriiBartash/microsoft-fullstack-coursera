using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Handler for updating an existing product.
/// </summary>
public sealed class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the product update request.
    /// </summary>
    public async Task<Result<bool>> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (product is null)
        {
            return Result<bool>.Failure("Product.NotFound", $"Product with ID '{request.Id}' was not found.");
        }

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId, ct);

        if (!categoryExists)
        {
            return Result<bool>.Failure("Product.CategoryNotFound", $"Category with ID '{request.CategoryId}' does not exist.");
        }

        if (request.SupplierId.HasValue)
        {
            var supplierExists = await _context.Suppliers
                .AnyAsync(s => s.Id == request.SupplierId.Value, ct);

            if (!supplierExists)
            {
                return Result<bool>.Failure("Product.SupplierNotFound", $"Supplier with ID '{request.SupplierId}' does not exist.");
            }
        }

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.MinimumStock,
            request.CategoryId,
            request.SupplierId);

        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}
