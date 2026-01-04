using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Categories.Commands;

/// <summary>
/// Command to create a new category
/// </summary>
public sealed record CreateCategoryCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Name of the category
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Optional description of the category
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Parent category identifier
    /// </summary>
    public Guid? ParentId { get; init; }
}
