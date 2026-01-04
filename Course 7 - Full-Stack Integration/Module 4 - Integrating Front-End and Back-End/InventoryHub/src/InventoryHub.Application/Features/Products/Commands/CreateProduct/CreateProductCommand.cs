using InventoryHub.Application.Common.Models;
using MediatR;

namespace InventoryHub.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Command to create a new product in the inventory system.
/// </summary>
public sealed record CreateProductCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Stock Keeping Unit - unique identifier for the product.
    /// </summary>
    public required string SKU { get; init; }

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

    /// <summary>
    /// Initial stock quantity. If greater than 0, creates a stock movement.
    /// </summary>
    public int InitialStock { get; init; }
}
