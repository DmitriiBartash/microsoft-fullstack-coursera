namespace InventoryHub.Domain.Enums;

/// <summary>
/// Status workflow for sales orders (client orders)
/// Draft → Confirmed → Shipped → Delivered (or Cancelled at any point except Delivered)
/// </summary>
public enum SalesOrderStatus
{
    Draft = 1,
    Confirmed = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}
