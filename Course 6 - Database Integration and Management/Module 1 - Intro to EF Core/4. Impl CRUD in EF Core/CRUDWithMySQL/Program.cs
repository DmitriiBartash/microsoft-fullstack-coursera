using System.Globalization;
using CRUDWithMySQL.Data;
using CRUDWithMySQL.Models;

var mdl = new CultureInfo("ro-MD");

using (var context = new ApplicationDbContext())
{
    // === CREATE ===
    var newProduct = new Product { Name = "Laptop", Price = 1200.50m };
    context.Products.Add(newProduct);
    context.SaveChanges();
    Console.WriteLine($"✅ Product created: {newProduct.Name}");

    // === READ ===
    var products = context.Products.ToList();
    Console.WriteLine("\n📋 All products:");
    foreach (var p in products)
        Console.WriteLine($"{p.Id}: {p.Name} - {p.Price.ToString("C", mdl)}");

    // === READ ===
    var product = context.Products.Find(newProduct.Id);
    if (product != null)
        Console.WriteLine($"\n🔍 Found product: {product.Name} - {product.Price.ToString("C", mdl)}");

    // === UPDATE ===
    if (product != null)
    {
        product.Price = 999.99m;
        context.SaveChanges();
        Console.WriteLine($"\n✏️ Updated product: {product.Name} - {product.Price.ToString("C", mdl)}");
    }

    // === DELETE ===
    if (product != null)
    {
        context.Products.Remove(product);
        context.SaveChanges();
        Console.WriteLine($"\n🗑 Deleted product: {product.Name}");
    }

    Console.WriteLine("\n📋 Final list:");
    foreach (var p in context.Products.ToList())
        Console.WriteLine($"{p.Id}: {p.Name} - {p.Price.ToString("C", mdl)}");
}
