using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Dashboard.Queries;

/// <summary>
/// Handler for retrieving products with low stock levels
/// </summary>
public sealed class GetLowStockProductsHandler : IRequestHandler<GetLowStockProductsQuery, Result<List<LowStockProductDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetLowStockProductsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LowStockProductDto>>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.IsActive);

        query = request.OnlyOutOfStock
            ? query.Where(p => p.CurrentStock == 0)
            : query.Where(p => p.CurrentStock <= p.MinimumStock);

        var products = await query
            .OrderBy(p => p.CurrentStock)
            .ThenBy(p => p.Name)
            .Take(request.Limit)
            .Select(p => new LowStockProductDto
            {
                Id = p.Id,
                SKU = p.SKU,
                Name = p.Name,
                CategoryName = p.Category.Name,
                CurrentStock = p.CurrentStock,
                MinimumStock = p.MinimumStock,
                StockDeficit = p.MinimumStock - p.CurrentStock,
                Price = p.Price,
                SupplierName = p.Supplier != null ? p.Supplier.Name : null,
                IsOutOfStock = p.CurrentStock == 0
            })
            .ToListAsync(cancellationToken);

        return Result<List<LowStockProductDto>>.Success(products);
    }
}
