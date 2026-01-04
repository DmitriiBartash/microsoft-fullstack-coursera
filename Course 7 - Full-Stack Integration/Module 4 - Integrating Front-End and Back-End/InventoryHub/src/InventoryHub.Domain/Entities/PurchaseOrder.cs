using InventoryHub.Domain.Enums;

namespace InventoryHub.Domain.Entities;

public class PurchaseOrder : BaseEntity
{
    public string OrderNumber { get; private set; } = null!;

    public Guid SupplierId { get; private set; }
    public Supplier Supplier { get; private set; } = null!;

    public OrderStatus Status { get; private set; } = OrderStatus.Draft;
    public DateTime OrderDate { get; private set; }
    public DateTime? ExpectedDeliveryDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }

    public ICollection<OrderItem> Items { get; private set; } = [];

    private PurchaseOrder() { }

    public static PurchaseOrder Create(
        string orderNumber,
        Guid supplierId,
        DateTime? expectedDeliveryDate = null,
        string? notes = null)
    {
        return new PurchaseOrder
        {
            OrderNumber = orderNumber,
            SupplierId = supplierId,
            OrderDate = DateTime.UtcNow,
            ExpectedDeliveryDate = expectedDeliveryDate,
            Notes = notes
        };
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        var item = OrderItem.Create(Id, productId, quantity, unitPrice);
        Items.Add(item);
        CalculateTotal();
    }

    public void RemoveItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            Items.Remove(item);
            CalculateTotal();
        }
    }

    public void CalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.LineTotal);
        SetUpdated();
    }

    public void Submit()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Only draft orders can be submitted");

        if (!Items.Any())
            throw new InvalidOperationException("Cannot submit an empty order");

        Status = OrderStatus.Submitted;
        SetUpdated();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Submitted)
            throw new InvalidOperationException("Only submitted orders can be confirmed");

        Status = OrderStatus.Confirmed;
        SetUpdated();
    }

    public void MarkShipped()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be marked as shipped");

        Status = OrderStatus.Shipped;
        SetUpdated();
    }

    public void MarkReceived()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be marked as received");

        Status = OrderStatus.Received;
        SetUpdated();
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Received)
            throw new InvalidOperationException("Cannot cancel a received order");

        Status = OrderStatus.Cancelled;
        SetUpdated();
    }

    /// <summary>
    /// Sets order status directly (for admin override).
    /// </summary>
    public void SetStatus(OrderStatus status)
    {
        Status = status;
        SetUpdated();
    }

    public void Update(DateTime? expectedDeliveryDate, string? notes)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Only draft orders can be updated");

        ExpectedDeliveryDate = expectedDeliveryDate;
        Notes = notes;
        SetUpdated();
    }
}
