using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Client.Models;

/// <summary>
/// Category data transfer object
/// </summary>
public class CategoryDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    public Guid? ParentId { get; set; }

    public string? ParentName { get; set; }

    public int ProductCount { get; set; }

    public List<CategoryDto> Children { get; set; } = [];

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Category creation request
/// </summary>
public class CreateCategoryDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}

/// <summary>
/// Category update request
/// </summary>
public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}

/// <summary>
/// Category creation request for service calls
/// </summary>
public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
}

/// <summary>
/// Category update request for service calls
/// </summary>
public class UpdateCategoryRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
}
