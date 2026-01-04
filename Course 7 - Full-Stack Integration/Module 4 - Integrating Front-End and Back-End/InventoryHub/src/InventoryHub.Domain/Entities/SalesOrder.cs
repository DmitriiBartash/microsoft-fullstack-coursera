using InventoryHub.Domain.Enums;

namespace InventoryHub.Domain.Entities;

/// <summary>
/// Sales order entity (order to client, triggers Stock Out on ship)
/// Workflow: Draft → Confirmed → Shipped → Delivered (or Cancelled)
/// </summary>
public class SalesOrder : BaseEntity
{
    public string OrderNumber { get; private set; } = null!;

    public Guid ClientId { get; private set; }
    public Client Client { get; private set; } = null!;

    public SalesOrderStatus Status { get; private set; } = SalesOrderStatus.Draft;
    public DateTime OrderDate { get; private set; }
    public DateTime? RequiredDate { get; private set; }
    public string? ShippingAddress { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }

    public ICollection<SalesOrderItem> Items { get; private set; } = [];

    private SalesOrder() { }

    public static SalesOrder Create(
        string orderNumber,
        Guid clientId,
        DateTime? requiredDate = null,
        string? shippingAddress = null,
        string? notes = null)
    {
        return new SalesOrder
        {
            OrderNumber = orderNumber,
            ClientId = clientId,
            OrderDate = DateTime.UtcNow,
            RequiredDate = requiredDate,
            ShippingAddress = shippingAddress,
            Notes = notes
        };
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        var item = SalesOrderItem.Create(Id, productId, quantity, unitPrice);
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

    /// <summary>
    /// Confirm the order (Draft → Confirmed)
    /// </summary>
    public void Confirm()
    {
        if (Status != SalesOrderStatus.Draft)
            throw new InvalidOperationException("Only draft orders can be confirmed");

        if (!Items.Any())
            throw new InvalidOperationException("Cannot confirm an empty order");

        Status = SalesOrderStatus.Confirmed;
        SetUpdated();
    }

    /// <summary>
    /// Ship the order (Confirmed → Shipped)
    /// Stock Out should be created by the handler before calling this method
    /// </summary>
    public void Ship()
    {
        if (Status != SalesOrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be shipped");

        Status = SalesOrderStatus.Shipped;
        SetUpdated();
    }

    /// <summary>
    /// Mark order as delivered (Shipped → Delivered)
    /// </summary>
    public void MarkDelivered()
    {
        if (Status != SalesOrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be marked as delivered");

        Status = SalesOrderStatus.Delivered;
        SetUpdated();
    }

    /// <summary>
    /// Cancel the order (any status except Delivered → Cancelled)
    /// </summary>
    public void Cancel()
    {
        if (Status == SalesOrderStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel a delivered order");

        if (Status == SalesOrderStatus.Shipped)
            throw new InvalidOperationException("Cannot cancel a shipped order (stock already deducted)");

        Status = SalesOrderStatus.Cancelled;
        SetUpdated();
    }

    /// <summary>
    /// Sets order status directly (for admin override)
    /// </summary>
    public void SetStatus(SalesOrderStatus status)
    {
        Status = status;
        SetUpdated();
    }

    /// <summary>
    /// Update order details (only for Draft orders)
    /// </summary>
    public void Update(DateTime? requiredDate, string? shippingAddress, string? notes)
    {
        if (Status != SalesOrderStatus.Draft)
            throw new InvalidOperationException("Only draft orders can be updated");

        RequiredDate = requiredDate;
        ShippingAddress = shippingAddress;
        Notes = notes;
        SetUpdated();
    }
}
