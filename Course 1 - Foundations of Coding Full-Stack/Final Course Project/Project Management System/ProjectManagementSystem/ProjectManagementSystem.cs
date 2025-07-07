class Product(string name, decimal price, int stock)
{
    public string Name { get; set; } = name;
    public decimal Price { get; set; } = price;
    public int Stock { get; set; } = stock;

    public void Display()
    {
        Console.WriteLine($"Name: {Name}, Price: ${Price}, Stock: {Stock}");
    }
}

class Program
{
    static readonly List<Product> inventory = [];

    static void Main()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\nInventory Management System");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. View Inventory");
            Console.WriteLine("3. Update Stock");
            Console.WriteLine("4. Remove Product");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option (1-5): ");
            string choice = Console.ReadLine() ?? string.Empty;

            switch (choice)
            {
                case "1":
                    AddProduct();
                    break;
                case "2":
                    ViewInventory();
                    break;
                case "3":
                    UpdateStock();
                    break;
                case "4":
                    RemoveProduct();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please choose between 1 and 5.");
                    break;
            }
        }

        Console.WriteLine("Exiting the program. Goodbye!");
    }

    static void AddProduct()
    {
        Console.Write("Enter product name: ");
        string name = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("Invalid price input.");
            return;
        }

        Console.Write("Enter stock quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int stock))
        {
            Console.WriteLine("Invalid stock input.");
            return;
        }

        inventory.Add(new Product(name, price, stock));
        Console.WriteLine("Product added successfully.");
    }

    static void ViewInventory()
    {
        if (inventory.Count == 0)
        {
            Console.WriteLine("Inventory is empty.");
        }
        else
        {
            Console.WriteLine("\nCurrent Inventory:");
            foreach (var product in inventory) 
            {
                product.Display();
            }
        }
    }

    static void UpdateStock()
    {
        Console.Write("Enter product name to update: ");
        string name = Console.ReadLine() ?? string.Empty;

        Product? found = inventory.Find(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (found != null)
        {
            Console.Write("Enter quantity to add (use negative number to subtract): ");
            if (!int.TryParse(Console.ReadLine(), out int quantityChange))
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            found.Stock += quantityChange;
            Console.WriteLine("Stock updated successfully.");
        }
        else
        {
            Console.WriteLine("Product not found.");
        }
    }

    static void RemoveProduct()
    {
        Console.Write("Enter product name to remove: ");
        string name = Console.ReadLine() ?? string.Empty;

        Product? found = inventory.Find(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (found != null)
        {
            inventory.Remove(found);
            Console.WriteLine("Product removed successfully.");
        }
        else
        {
            Console.WriteLine("Product not found.");
        }
    }
}
