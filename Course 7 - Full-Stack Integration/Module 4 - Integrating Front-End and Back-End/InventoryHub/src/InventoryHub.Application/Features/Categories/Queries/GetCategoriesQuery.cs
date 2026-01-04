using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Categories.Queries;

/// <summary>
/// Query to retrieve all categories with hierarchy
/// </summary>
public sealed record GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>
{
    /// <summary>
    /// Include child categories in the result
    /// </summary>
    public bool IncludeChildren { get; init; } = true;

    /// <summary>
    /// Include product count for each category
    /// </summary>
    public bool IncludeProductCount { get; init; } = true;
}
