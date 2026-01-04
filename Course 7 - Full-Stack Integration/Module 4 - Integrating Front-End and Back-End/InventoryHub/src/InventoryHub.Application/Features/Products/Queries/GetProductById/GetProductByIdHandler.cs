using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Handler for retrieving a product by ID with navigation properties
/// </summary>
public sealed class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken ct)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Movements.OrderByDescending(m => m.CreatedAt).Take(10))
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (product is null)
        {
            return Result<ProductDto>.Failure(
                "Product.NotFound",
                $"Product with ID '{request.Id}' was not found.");
        }

        var productDto = product.Adapt<ProductDto>();

        return Result<ProductDto>.Success(productDto);
    }
}
