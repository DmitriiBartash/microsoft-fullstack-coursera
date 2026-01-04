namespace InventoryHub.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? ParentId { get; private set; }

    public Category? Parent { get; private set; }
    public ICollection<Category> Children { get; private set; } = [];
    public ICollection<Product> Products { get; private set; } = [];

    private Category() { }

    public static Category Create(string name, string? description = null, Guid? parentId = null)
    {
        return new Category
        {
            Name = name,
            Description = description,
            ParentId = parentId
        };
    }

    public void Update(string name, string? description, Guid? parentId)
    {
        Name = name;
        Description = description;
        ParentId = parentId;
        SetUpdated();
    }
}
