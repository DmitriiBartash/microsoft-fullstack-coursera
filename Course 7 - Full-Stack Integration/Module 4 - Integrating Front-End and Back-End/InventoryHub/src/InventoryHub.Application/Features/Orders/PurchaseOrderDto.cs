namespace InventoryHub.Application.Features.Orders;

/// <summary>
/// Data transfer object for purchase order.
/// </summary>
public sealed class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
}
