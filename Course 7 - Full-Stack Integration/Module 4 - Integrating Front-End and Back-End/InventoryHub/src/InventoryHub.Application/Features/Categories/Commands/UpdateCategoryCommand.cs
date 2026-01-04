using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Categories.Commands;

/// <summary>
/// Command to update an existing category
/// </summary>
public sealed record UpdateCategoryCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Category identifier
    /// </summary>
    public Guid Id { get; init; }

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
