using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Command to update an existing product.
/// </summary>
public sealed record UpdateProductCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Product identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Product name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Optional product description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Product price.
    /// </summary>
    public required decimal Price { get; init; }

    /// <summary>
    /// Minimum stock level that triggers low stock alert.
    /// </summary>
    public required int MinimumStock { get; init; }

    /// <summary>
    /// Category identifier.
    /// </summary>
    public required Guid CategoryId { get; init; }

    /// <summary>
    /// Optional supplier identifier.
    /// </summary>
    public Guid? SupplierId { get; init; }
}
