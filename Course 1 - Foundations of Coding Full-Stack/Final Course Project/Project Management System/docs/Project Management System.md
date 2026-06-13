# Project Management System

**Course 1 — Foundations of Coding Full-Stack** · `Project`

> Final-project capstone: build a **console-based Inventory Management System** in C# from
> scratch. Users add, view, update, and remove products; the app keeps a menu running in a
> loop and stores everything in memory at runtime. This is your first program written
> end-to-end on your own — it ties together classes, control structures, loops, and methods.

---

## 🎯 Objective

Design and implement a working, menu-driven inventory application in a **C# console**
environment that lets users **add**, **view**, **update**, and **remove** products — applying
object-oriented principles, control structures, loops, and function decomposition to reach a
runnable result.

---

## 🗂️ What you will build

A single C# console project (suggested name **`InventoryManagementSystem`**) with two classes:

| Component        | Responsibility                                                            |
| ---------------- | ------------------------------------------------------------------------- |
| `Product`        | Model a product: `Name` (string), `Price` (decimal), `Stock` (int); display itself |
| `Program`        | Menu loop plus the four operation methods that drive the inventory        |
| `AddProduct()`   | Read name, price, stock; create a `Product` and store it                  |
| `ViewInventory()`| List every product and its current stock level                            |
| `UpdateStock()`  | Find a product by name and adjust its stock (positive or negative)        |
| `RemoveProduct()`| Find a product by name and remove it from the list                        |

**Flow:** `menu → switch(choice) → AddProduct / ViewInventory / UpdateStock / RemoveProduct → loop until Exit`

---

## 📋 Requirements and objectives

You define what you are solving before you build it. Requirements fall into two categories —
functional (what the system does) and non-functional (how it behaves) — followed by the
objectives you aim to achieve.

**Functional requirements**

- Add new products with **Name** (string), **Price** (decimal), and **Stock quantity** (integer).
- View all products, displaying name, price, and current stock.
- Update stock levels when items are sold or restocked (positive **or** negative adjustment).
- Remove a product from inventory by name.
- Present a menu of options in a loop until the user chooses to exit.

**Non-functional requirements**

- The system is a **console application written in C#**.
- Handle user input errors gracefully (e.g., invalid menu option or wrong data type).
- Keep code modular — a separate method for each major function (e.g., `AddProduct`, `UpdateStock`).
- Be easy to use, with clear instructions and labels.

**Project objectives**

- Design and implement a working inventory system using object-oriented principles.
- Demonstrate control structures (`if-else`, `switch`) for decision-making logic.
- Use loop constructs (`while`, `for`) to support ongoing interaction.
- Apply function decomposition by defining and calling custom methods.
- Practice structured programming and basic user interaction in a C# console environment.

---

## 🧭 Design outline

Plan the major tasks and the code component each one needs before writing it.

| Task                  | Description                                              | Code component needed              |
| --------------------- | ------------------------------------------------------- | ---------------------------------- |
| Display menu          | Show options (Add, View, Update, Remove, Exit)          | `switch` or `if-else` in a loop    |
| Add new product       | Get name, price, stock; create and store a product      | `AddProduct()` + variables         |
| View inventory        | Display all products in the list                        | `ViewInventory()` + `foreach`      |
| Update product stock  | Search by name, adjust stock (add or subtract)          | `UpdateStock()` + conditionals     |
| Remove product        | Search by name, remove from list                        | `RemoveProduct()` + list methods   |
| Keep program running  | Loop through menu until exit is chosen                  | `while` / `do-while` loop          |
| Handle errors         | Validate input, display error messages                  | `TryParse` / `if-else`             |

Planned variables:

| Variable name | Type             | Purpose                          |
| ------------- | ---------------- | -------------------------------- |
| `inventory`   | `List<Product>`  | Stores all products in memory    |
| `name`        | `string`         | Holds product name input         |
| `price`       | `decimal`        | Holds product price input        |
| `stock`       | `int`            | Holds quantity input             |
| `choice`      | `string`         | Holds user menu selection        |

> *(The original includes a flowchart — "Picture 1: Inventory Management System Flowchart" — that
> walks the menu loop: display options → branch on the user's choice → perform the action →
> ask whether to continue or exit.)*

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (Copilot is optional and may assist while you code)
- Basic familiarity with C# classes, `switch`/`if-else`, loops, and methods

---

## 🛠️ Steps

### Step 1 — Scaffold the console project

Create the project and move into it.

```bash
dotnet new console -n InventoryManagementSystem
cd InventoryManagementSystem
```

### Step 2 — Define the `Product` model

Create **`Product.cs`**. The class holds the three product fields and can print itself, using a
primary constructor so the fields are populated at creation.

```csharp
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
```

### Step 3 — Build the menu loop in `Main`

In **`Program.cs`**, keep the inventory in a `List<Product>` and loop over a menu with a
`switch` statement until the user chooses **Exit (5)**.

```csharp
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
```

### Step 4 — Add a product

`AddProduct()` reads input and validates the numeric fields with `TryParse`, returning early on
bad input so the program never crashes.

```csharp
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
```

### Step 5 — View the inventory

`ViewInventory()` handles the empty case, then prints each product via `Product.Display()`.

```csharp
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
```

### Step 6 — Update stock

`UpdateStock()` finds a product by name (case-insensitive) and applies a signed adjustment —
positive to restock, negative to record a sale.

```csharp
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
```

### Step 7 — Remove a product

`RemoveProduct()` looks the product up the same way and removes it from the list.

```csharp
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
```

### Step 8 — Run, then save and submit

Run the app and exercise every menu option, then save your project outline and code and paste
the relevant sections into the submission prompts.

```bash
dotnet run
```

---

## ▶️ Expected result

The program prints the menu and loops on each choice. **Add** stores a product, **View** lists
every product (`Name: ..., Price: $..., Stock: ...`), **Update** adjusts a found product's
stock, **Remove** deletes it, and invalid menu input prints `Invalid option. Please choose
between 1 and 5.` Choosing **5** prints `Exiting the program. Goodbye!` and ends the loop.

---

## ☑️ Definition of done

- [ ] Console project created and `Product` modeled with `Name`, `Price`, `Stock`
- [ ] Menu loop runs via a `switch` over options 1–5 until **Exit**
- [ ] `AddProduct()`, `ViewInventory()`, `UpdateStock()`, `RemoveProduct()` implemented and called
- [ ] Numeric input validated with `TryParse`; invalid input handled gracefully
- [ ] Functional/non-functional requirements and objectives written up
- [ ] `dotnet run` works end-to-end; outline and code saved and submitted

---

## 🔑 Key concepts

- **Object-oriented modeling** — `Product` bundles data (`Name`, `Price`, `Stock`) with behavior
  (`Display()`), so the rest of the program works with whole products instead of loose variables.
- **Function decomposition** — each menu action is its own method (`AddProduct`, `ViewInventory`,
  `UpdateStock`, `RemoveProduct`), keeping `Main` a thin dispatcher that is easy to read and change.
- **Control flow drives the UX** — a `while` loop sustains interaction and a `switch` routes the
  user's choice; the program only stops when the user explicitly exits.
- **Validate input, fail gracefully** — `decimal.TryParse` / `int.TryParse` reject bad data and
  return early, so a typo never crashes the app.
- **Requirements before code** — separating functional from non-functional requirements and
  setting objectives turns a vague brief into a checklist you can actually build against.
