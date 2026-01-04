namespace InventoryHub.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public PurchaseOrder Order { get; private set; } = null!;

    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int ReceivedQuantity { get; private set; }

    public decimal LineTotal => Quantity * UnitPrice;
    public bool IsFullyReceived => ReceivedQuantity >= Quantity;

    private OrderItem() { }

    public static OrderItem Create(Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        return new OrderItem
        {
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            ReceivedQuantity = 0
        };
    }

    public void UpdateQuantity(int quantity, decimal unitPrice)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        SetUpdated();
    }

    public void RecordReceived(int quantity)
    {
        ReceivedQuantity += quantity;
        if (ReceivedQuantity > Quantity)
            ReceivedQuantity = Quantity;
        SetUpdated();
    }
}
