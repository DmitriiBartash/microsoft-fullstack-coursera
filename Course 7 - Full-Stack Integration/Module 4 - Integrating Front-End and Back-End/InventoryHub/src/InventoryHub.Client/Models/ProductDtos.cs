using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Client.Models;

/// <summary>
/// Product data transfer object
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public Guid? SupplierId { get; set; }

    public string? SupplierName { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Current stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Current stock cannot be negative")]
    public int CurrentStock { get; set; }

    [Required(ErrorMessage = "Minimum stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Minimum stock cannot be negative")]
    public int MinimumStock { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsOutOfStock => CurrentStock == 0;
    public bool IsLowStock => CurrentStock > 0 && CurrentStock <= MinimumStock;
    public string StockStatus => IsOutOfStock ? "Out of Stock" : IsLowStock ? "Low Stock" : "In Stock";
}

/// <summary>
/// Product creation request
/// </summary>
public class CreateProductDto
{
    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    public Guid? SupplierId { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Initial stock cannot be negative")]
    public int InitialStock { get; set; }

    [Required(ErrorMessage = "Minimum stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Minimum stock cannot be negative")]
    public int MinimumStock { get; set; }
}

/// <summary>
/// Product update request
/// </summary>
public class UpdateProductDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    public Guid? SupplierId { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Minimum stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Minimum stock cannot be negative")]
    public int MinimumStock { get; set; }
}

/// <summary>
/// Product creation request for service calls
/// </summary>
public class CreateProductRequest
{
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? SupplierId { get; set; }
    public decimal Price { get; set; }
    public int InitialStock { get; set; }
    public int MinimumStock { get; set; }
}

/// <summary>
/// Product update request for service calls
/// </summary>
public class UpdateProductRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? SupplierId { get; set; }
    public decimal Price { get; set; }
    public int MinimumStock { get; set; }
}
