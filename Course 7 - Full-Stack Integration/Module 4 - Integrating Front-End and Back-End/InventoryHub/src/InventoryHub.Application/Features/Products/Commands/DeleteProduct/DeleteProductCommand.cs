using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Command to soft delete a product by deactivating it.
/// </summary>
public sealed record DeleteProductCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Product identifier to delete.
    /// </summary>
    public required Guid Id { get; init; }
}
