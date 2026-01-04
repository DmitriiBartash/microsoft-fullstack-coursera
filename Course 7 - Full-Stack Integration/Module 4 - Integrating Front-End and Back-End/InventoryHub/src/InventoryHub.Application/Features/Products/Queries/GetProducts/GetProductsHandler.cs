using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Application.Common.Models;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Application.Features.Products.Queries.GetProducts;

/// <summary>
/// Handler for retrieving paginated products with filters
/// </summary>
public sealed class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<PaginatedList<ProductDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ProductDto>>> Handle(
        GetProductsQuery request,
        CancellationToken ct)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .AsQueryable();

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
        }
        else
        {
            query = query.Where(p => p.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower().Trim();
            var cleanedSearchTerm = searchTerm.Replace("$", "").Replace(",", "").Trim();

            if (!string.IsNullOrWhiteSpace(cleanedSearchTerm))
            {
                var likePattern = $"%{cleanedSearchTerm}%";
                var pricePrefixPattern = $"{cleanedSearchTerm}%";

                query = query.Where(prod =>
                    EF.Functions.Like(prod.Name.ToLower(), likePattern) ||
                    EF.Functions.Like(prod.SKU.ToLower(), likePattern) ||
                    (prod.Description != null && EF.Functions.Like(prod.Description.ToLower(), likePattern)) ||
                    (prod.Category != null && EF.Functions.Like(prod.Category.Name.ToLower(), likePattern)) ||
                    (prod.Supplier != null && EF.Functions.Like(prod.Supplier.Name.ToLower(), likePattern)) ||
                    EF.Functions.Like(prod.Price.ToString(), pricePrefixPattern) ||
                    EF.Functions.Like(prod.CurrentStock.ToString(), likePattern));
            }
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        if (request.SupplierId.HasValue)
        {
            query = query.Where(p => p.SupplierId == request.SupplierId.Value);
        }

        if (request.LowStock.HasValue && request.LowStock.Value)
        {
            query = query.Where(p => p.CurrentStock <= p.MinimumStock);
        }

        var isDescending = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        query = request.SortBy?.ToLowerInvariant() switch
        {
            "sku" => isDescending ? query.OrderByDescending(p => p.SKU) : query.OrderBy(p => p.SKU),
            "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "category" => isDescending
                ? query.OrderByDescending(p => p.Category != null ? p.Category.Name : string.Empty)
                : query.OrderBy(p => p.Category != null ? p.Category.Name : string.Empty),
            "price" => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "stock" => isDescending ? query.OrderByDescending(p => p.CurrentStock) : query.OrderBy(p => p.CurrentStock),
            _ => query.OrderBy(p => p.Name)
        };

        var projectedQuery = query.ProjectToType<ProductDto>();

        var paginatedList = await PaginatedList<ProductDto>.CreateAsync(
            projectedQuery,
            request.Page,
            request.PageSize,
            ct);

        return Result<PaginatedList<ProductDto>>.Success(paginatedList);
    }
}
