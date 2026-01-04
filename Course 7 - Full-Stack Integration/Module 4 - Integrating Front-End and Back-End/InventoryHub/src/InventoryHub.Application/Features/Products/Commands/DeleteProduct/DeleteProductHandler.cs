using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Handler for soft deleting a product.
/// </summary>
public sealed class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the product deletion request by deactivating the product.
    /// </summary>
    public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (product is null)
        {
            return Result<bool>.Failure("Product.NotFound", $"Product with ID '{request.Id}' was not found.");
        }

        if (!product.IsActive)
        {
            return Result<bool>.Failure("Product.AlreadyDeactivated", "Product is already deactivated.");
        }

        product.Deactivate();
        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}
