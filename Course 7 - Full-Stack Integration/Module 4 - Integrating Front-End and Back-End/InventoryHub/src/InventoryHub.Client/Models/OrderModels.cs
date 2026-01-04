namespace InventoryHub.Client.Models;

/// <summary>
/// Order data transfer object
/// </summary>
public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

/// <summary>
/// Order item data transfer object
/// </summary>
public class OrderItemDto
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

/// <summary>
/// Order status constants (matches Domain.Enums.OrderStatus)
/// </summary>
public static class OrderStatus
{
    public const string Draft = "Draft";
    public const string Submitted = "Submitted";
    public const string Confirmed = "Confirmed";
    public const string Shipped = "Shipped";
    public const string Received = "Received";
    public const string Cancelled = "Cancelled";
}

/// <summary>
/// Request model for creating a new order
/// </summary>
public class CreateOrderRequest
{
    public Guid SupplierId { get; set; }
    public string? Notes { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Request model for creating an order item
/// </summary>
public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

/// <summary>
/// Request model for submitting an order
/// </summary>
public class SubmitOrderRequest
{
    public Guid OrderId { get; set; }
}

/// <summary>
/// Request model for receiving an order
/// </summary>
public class ReceiveOrderRequest
{
    public Guid OrderId { get; set; }
}
