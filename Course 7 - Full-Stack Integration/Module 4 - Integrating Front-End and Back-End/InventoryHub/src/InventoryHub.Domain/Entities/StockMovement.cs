using InventoryHub.Domain.Enums;

namespace InventoryHub.Domain.Entities;

public class StockMovement : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public MovementType Type { get; private set; }
    public int Quantity { get; private set; }
    public int PreviousStock { get; private set; }
    public int NewStock { get; private set; }

    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public string? CreatedBy { get; private set; }

    private StockMovement() { }

    public static StockMovement Create(
        Guid productId,
        MovementType type,
        int quantity,
        int previousStock,
        int newStock,
        string? reference = null,
        string? notes = null,
        string? createdBy = null)
    {
        return new StockMovement
        {
            ProductId = productId,
            Type = type,
            Quantity = Math.Abs(quantity),
            PreviousStock = previousStock,
            NewStock = newStock,
            Reference = reference,
            Notes = notes,
            CreatedBy = createdBy
        };
    }
}
