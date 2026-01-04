using InventoryHub.Domain.Enums;

namespace InventoryHub.Domain.Entities;

public class Product : BaseEntity
{
    public string SKU { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int CurrentStock { get; private set; }
    public int MinimumStock { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public Guid? SupplierId { get; private set; }
    public Supplier? Supplier { get; private set; }

    public ICollection<StockMovement> Movements { get; private set; } = [];

    public bool IsLowStock => CurrentStock <= MinimumStock;
    public bool IsOutOfStock => CurrentStock == 0;

    private Product() { }

    public static Product Create(
        string sku,
        string name,
        decimal price,
        int minimumStock,
        Guid categoryId,
        string? description = null,
        Guid? supplierId = null)
    {
        return new Product
        {
            SKU = sku,
            Name = name,
            Description = description,
            Price = price,
            MinimumStock = minimumStock,
            CategoryId = categoryId,
            SupplierId = supplierId,
            CurrentStock = 0
        };
    }

    public void Update(
        string name,
        string? description,
        decimal price,
        int minimumStock,
        Guid categoryId,
        Guid? supplierId)
    {
        Name = name;
        Description = description;
        Price = price;
        MinimumStock = minimumStock;
        CategoryId = categoryId;
        SupplierId = supplierId;
        SetUpdated();
    }

    public StockMovement AdjustStock(int quantity, MovementType type, string? reference = null, string? notes = null, string? createdBy = null)
    {
        var previousStock = CurrentStock;

        CurrentStock = type switch
        {
            MovementType.In => CurrentStock + quantity,
            MovementType.Out => CurrentStock - quantity,
            MovementType.Adjust => quantity,
            _ => CurrentStock
        };

        if (CurrentStock < 0)
            CurrentStock = 0;

        SetUpdated();

        return StockMovement.Create(
            Id,
            type,
            type == MovementType.Adjust ? CurrentStock - previousStock : quantity,
            previousStock,
            CurrentStock,
            reference,
            notes,
            createdBy);
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdated();
    }
}
