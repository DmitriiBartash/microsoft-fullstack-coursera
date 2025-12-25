using BlazorStateApp.Shared.Models;

namespace BlazorStateApp.Shared;

/// <summary>
/// Centralized product catalog.
/// </summary>
public static class ProductCatalog
{
    private static readonly Dictionary<string, Product> ProductsByName;

    public static IReadOnlyList<Product> All { get; } = new Product[]
    {
        new("Laptop", 999.99m, "Electronics", "💻"),
        new("Headphones", 149.99m, "Electronics", "🎧"),
        new("Keyboard", 79.99m, "Electronics", "⌨️"),
        new("Mouse", 49.99m, "Electronics", "🖱️"),
        new("Monitor", 299.99m, "Electronics", "🖥️"),
        new("Webcam", 89.99m, "Electronics", "📷"),
        new("Coffee Mug", 14.99m, "Office", "☕"),
        new("Notebook", 9.99m, "Office", "📓"),
        new("Desk Lamp", 34.99m, "Office", "💡"),
    };

    static ProductCatalog()
    {
        ProductsByName = All.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets product by name. O(1) lookup.
    /// </summary>
    public static Product? GetByName(string name) =>
        ProductsByName.TryGetValue(name, out var product) ? product : null;
}
