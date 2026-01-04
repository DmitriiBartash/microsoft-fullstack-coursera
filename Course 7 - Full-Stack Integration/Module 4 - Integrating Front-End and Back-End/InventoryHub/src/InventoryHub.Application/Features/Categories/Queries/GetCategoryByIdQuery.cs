using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Categories.Queries;

/// <summary>
/// Query to retrieve a specific category by identifier
/// </summary>
public sealed record GetCategoryByIdQuery : IRequest<Result<CategoryDto>>
{
    /// <summary>
    /// Category identifier
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Include child categories in the result
    /// </summary>
    public bool IncludeChildren { get; init; } = true;

    public GetCategoryByIdQuery(Guid id)
    {
        Id = id;
    }
}
