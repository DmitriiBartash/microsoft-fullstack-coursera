namespace InventoryHub.Application.Features.Products;

/// <summary>
/// Product data transfer object
/// </summary>
public sealed class ProductDto
{
    public Guid Id { get; init; }
    public string SKU { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int CurrentStock { get; init; }
    public int MinimumStock { get; init; }
    public bool IsActive { get; init; }
    public bool IsLowStock { get; init; }
    public bool IsOutOfStock { get; init; }

    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = null!;

    public Guid? SupplierId { get; init; }
    public string? SupplierName { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
