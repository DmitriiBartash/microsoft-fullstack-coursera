using backend.Models;
using System.Text.Json;

namespace backend.Services;

public class ProductService
{
    private readonly List<Product> _products =
    [
        new() { Id = 1, Name = "Laptop Pro 15", Price = 1899.99m, Category = "Computers" },
        new() { Id = 2, Name = "Wireless Headphones", Price = 299.99m, Category = "Audio" },
        new() { Id = 3, Name = "Gaming Mouse", Price = 89.99m, Category = "Accessories" }
    ];

    private readonly object _lock = new();

    public IEnumerable<Product> GetAll() => _products.OrderBy(p => p.Id);

    public Product? GetById(int id) =>
        _products.FirstOrDefault(p => p.Id == id);

    public IEnumerable<Product> Search(string? keyword = null, string? category = null)
    {
        var query = _products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        return query.ToList();
    }

    public Product Add(Product newProduct)
    {
        lock (_lock)
        {
            newProduct.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            _products.Add(newProduct);
            return newProduct;
        }
    }

    public Product? Update(int id, Product updated)
    {
        lock (_lock)
        {
            var existing = _products.FirstOrDefault(p => p.Id == id);
            if (existing == null) return null;

            existing.Name = updated.Name;
            existing.Price = updated.Price;
            existing.Category = updated.Category;

            return existing;
        }
    }

    public Product? Patch(int id, JsonElement patchDoc)
    {
        lock (_lock)
        {
            var existing = _products.FirstOrDefault(p => p.Id == id);
            if (existing == null) return null;

            if (patchDoc.TryGetProperty("name", out var nameProp))
                existing.Name = nameProp.GetString() ?? existing.Name;

            if (patchDoc.TryGetProperty("price", out var priceProp) && priceProp.TryGetDecimal(out var price))
                existing.Price = price;

            if (patchDoc.TryGetProperty("category", out var categoryProp))
                existing.Category = categoryProp.GetString() ?? existing.Category;

            return existing;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return false;

            _products.Remove(product);
            return true;
        }
    }
}
