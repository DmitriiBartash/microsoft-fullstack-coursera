namespace InventoryHub.Client.Models;

/// <summary>
/// Sales order data transfer object
/// </summary>
public class SalesOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string? ShippingAddress { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public List<SalesOrderItemDto> Items { get; set; } = [];
}

/// <summary>
/// Sales order item data transfer object
/// </summary>
public class SalesOrderItemDto
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

/// <summary>
/// Sales order status constants (matches Domain.Enums.SalesOrderStatus)
/// </summary>
public static class SalesOrderStatus
{
    public const string Draft = "Draft";
    public const string Confirmed = "Confirmed";
    public const string Shipped = "Shipped";
    public const string Delivered = "Delivered";
    public const string Cancelled = "Cancelled";
}

/// <summary>
/// Request model for creating a new sales order
/// </summary>
public class CreateSalesOrderRequest
{
    public Guid ClientId { get; set; }
    public DateTime? RequiredDate { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public List<CreateSalesOrderItemRequest> Items { get; set; } = [];
}

/// <summary>
/// Request model for creating a sales order item
/// </summary>
public class CreateSalesOrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
