namespace InventoryHub.Application.Features.Clients;

/// <summary>
/// Data transfer object for client information.
/// </summary>
public class ClientDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? ContactPerson { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? ShippingAddress { get; init; }
    public bool IsActive { get; init; }
    public int OrderCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
