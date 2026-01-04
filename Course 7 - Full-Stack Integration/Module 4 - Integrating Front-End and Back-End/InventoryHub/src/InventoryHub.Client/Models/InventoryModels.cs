using System.Text.Json.Serialization;

namespace InventoryHub.Client.Models;

/// <summary>
/// Inventory movement data transfer object
/// </summary>
public class InventoryMovementDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MovementType Type { get; set; }
    public string TypeDisplay { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Movement type enumeration (matches API Domain enum)
/// </summary>
public enum MovementType
{
    In = 1,
    Out = 2,
    Adjust = 3
}

/// <summary>
/// Request model for recording an inventory movement
/// </summary>
public class RecordMovementRequest
{
    public Guid ProductId { get; set; }
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}
