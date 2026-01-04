namespace InventoryHub.Domain.Entities;

/// <summary>
/// Line item for a sales order (product being sold to client)
/// </summary>
public class SalesOrderItem : BaseEntity
{
    public Guid SalesOrderId { get; private set; }
    public SalesOrder SalesOrder { get; private set; } = null!;

    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal LineTotal => Quantity * UnitPrice;

    private SalesOrderItem() { }

    public static SalesOrderItem Create(Guid salesOrderId, Guid productId, int quantity, decimal unitPrice)
    {
        return new SalesOrderItem
        {
            SalesOrderId = salesOrderId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    public void UpdateQuantity(int quantity, decimal unitPrice)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        SetUpdated();
    }
}
