namespace InventoryHub.Application.Features.Suppliers;

/// <summary>
/// Data transfer object for supplier information.
/// </summary>
public class SupplierDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? ContactPerson { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public bool IsActive { get; init; }
    public int ProductCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
