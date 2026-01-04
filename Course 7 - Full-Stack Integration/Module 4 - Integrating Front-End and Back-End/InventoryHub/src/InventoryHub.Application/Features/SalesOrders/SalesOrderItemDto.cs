namespace InventoryHub.Application.Features.SalesOrders;

/// <summary>
/// Data transfer object for sales order item.
/// </summary>
public sealed class SalesOrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public int CurrentStock { get; set; }
}
