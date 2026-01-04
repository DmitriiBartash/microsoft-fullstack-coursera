using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Categories.Commands;

/// <summary>
/// Command to delete a category
/// </summary>
public sealed record DeleteCategoryCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Category identifier
    /// </summary>
    public Guid Id { get; init; }

    public DeleteCategoryCommand(Guid id)
    {
        Id = id;
    }
}
