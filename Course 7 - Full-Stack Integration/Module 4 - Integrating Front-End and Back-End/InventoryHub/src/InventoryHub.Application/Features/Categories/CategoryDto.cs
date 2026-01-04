namespace InventoryHub.Application.Features.Categories;

/// <summary>
/// Data transfer object for category information
/// </summary>
public sealed record CategoryDto
{
    /// <summary>
    /// Unique identifier of the category
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

    /// <summary>
    /// Parent category name
    /// </summary>
    public string? ParentName { get; init; }

    /// <summary>
    /// Number of products in this category
    /// </summary>
    public int ProductCount { get; init; }

    /// <summary>
    /// Child categories
    /// </summary>
    public List<CategoryDto> Children { get; init; } = [];

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}
