using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Dashboard.Queries;

/// <summary>
/// Query to retrieve products with low stock levels
/// </summary>
public sealed record GetLowStockProductsQuery : IRequest<Result<List<LowStockProductDto>>>
{
    /// <summary>
    /// Maximum number of products to return. Default is 50.
    /// </summary>
    public int Limit { get; init; } = 50;

    /// <summary>
    /// Include only out of stock products (CurrentStock = 0)
    /// </summary>
    public bool OnlyOutOfStock { get; init; }
}
