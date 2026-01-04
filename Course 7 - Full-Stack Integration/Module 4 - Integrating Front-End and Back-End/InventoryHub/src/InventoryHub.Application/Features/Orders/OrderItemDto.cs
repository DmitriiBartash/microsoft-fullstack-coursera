namespace InventoryHub.Application.Features.Orders;

/// <summary>
/// Data transfer object for order item.
/// </summary>
public sealed class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public int ReceivedQuantity { get; set; }
}
