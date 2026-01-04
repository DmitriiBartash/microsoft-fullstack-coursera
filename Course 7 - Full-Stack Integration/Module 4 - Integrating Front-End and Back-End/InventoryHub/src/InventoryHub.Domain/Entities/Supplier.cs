namespace InventoryHub.Domain.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string? ContactPerson { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<Product> Products { get; private set; } = [];
    public ICollection<PurchaseOrder> Orders { get; private set; } = [];

    private Supplier() { }

    public static Supplier Create(
        string name,
        string? contactPerson = null,
        string? email = null,
        string? phone = null,
        string? address = null)
    {
        return new Supplier
        {
            Name = name,
            ContactPerson = contactPerson,
            Email = email,
            Phone = phone,
            Address = address
        };
    }

    public void Update(
        string name,
        string? contactPerson,
        string? email,
        string? phone,
        string? address)
    {
        Name = name;
        ContactPerson = contactPerson;
        Email = email;
        Phone = phone;
        Address = address;
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
