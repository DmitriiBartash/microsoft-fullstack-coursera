namespace InventoryHub.Application.Features.SalesOrders;

/// <summary>
/// Data transfer object for sales order.
/// </summary>
public sealed class SalesOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public string? ShippingAddress { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public List<SalesOrderItemDto> Items { get; set; } = [];
}
