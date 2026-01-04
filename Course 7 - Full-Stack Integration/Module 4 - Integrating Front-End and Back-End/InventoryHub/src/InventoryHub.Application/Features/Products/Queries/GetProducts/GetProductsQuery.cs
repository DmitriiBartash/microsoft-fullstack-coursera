using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Products.Queries.GetProducts;

/// <summary>
/// Query to retrieve paginated list of products with optional filters and sorting
/// </summary>
public sealed record GetProductsQuery : IRequest<Result<PaginatedList<ProductDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? SupplierId { get; init; }
    public bool? LowStock { get; init; }
    public bool? IsActive { get; init; }
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
}
