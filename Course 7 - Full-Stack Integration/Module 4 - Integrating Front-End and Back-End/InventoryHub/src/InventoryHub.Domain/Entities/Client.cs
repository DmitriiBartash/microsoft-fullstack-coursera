namespace InventoryHub.Domain.Entities;

/// <summary>
/// Client entity for sales orders (customers receiving goods)
/// </summary>
public class Client : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string? ContactPerson { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public string? ShippingAddress { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<SalesOrder> Orders { get; private set; } = [];

    private Client() { }

    public static Client Create(
        string name,
        string? contactPerson = null,
        string? email = null,
        string? phone = null,
        string? address = null,
        string? shippingAddress = null)
    {
        return new Client
        {
            Name = name,
            ContactPerson = contactPerson,
            Email = email,
            Phone = phone,
            Address = address,
            ShippingAddress = shippingAddress
        };
    }

    public void Update(
        string name,
        string? contactPerson,
        string? email,
        string? phone,
        string? address,
        string? shippingAddress)
    {
        Name = name;
        ContactPerson = contactPerson;
        Email = email;
        Phone = phone;
        Address = address;
        ShippingAddress = shippingAddress;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdated();
    }
}
