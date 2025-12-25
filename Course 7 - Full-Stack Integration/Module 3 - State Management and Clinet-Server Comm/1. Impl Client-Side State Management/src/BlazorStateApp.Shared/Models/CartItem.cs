namespace BlazorStateApp.Shared.Models;

/// <summary>
/// Model for cart item.
/// Record provides immutability and value-based equality.
/// </summary>
public record CartItem
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; } = 1;
    public DateTime AddedAt { get; init; } = DateTime.UtcNow;
    public string? ImageUrl { get; init; }
    public string? Category { get; init; }

    /// <summary>Computed property - item total</summary>
    public decimal Total => Price * Quantity;
}
