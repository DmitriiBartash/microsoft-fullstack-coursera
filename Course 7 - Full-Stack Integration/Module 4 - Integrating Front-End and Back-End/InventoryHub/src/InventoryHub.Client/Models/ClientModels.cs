namespace InventoryHub.Client.Models;

/// <summary>
/// Client data transfer object
/// </summary>
public class ClientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? ShippingAddress { get; set; }
    public bool IsActive { get; set; }
    public int OrderCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Request model for creating a new client
/// </summary>
public class CreateClientRequest
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? ShippingAddress { get; set; }
}

/// <summary>
/// Request model for updating an existing client
/// </summary>
public class UpdateClientRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? ShippingAddress { get; set; }
    public bool IsActive { get; set; }
}
