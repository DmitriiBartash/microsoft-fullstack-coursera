# Implementing CRUD Operations in EF Core

**Course 6 — Database Integration and Management** · Module 1 · Lesson 4 · `You Try It!`

> Build a small .NET console app that performs the four basic data operations —
> **Create, Read, Update, Delete** — against a MySQL database using **Entity Framework
> Core**. A `Product` entity is mapped to a table, a migration builds the schema, and
> `Program.cs` exercises every CRUD path end-to-end.

---

## 🎯 Objective

By the end of this activity you will be able to perform basic **CRUD** (Create, Read,
Update, Delete) operations using **Entity Framework Core** with a **MySQL** database in a
.NET console application on Linux (Ubuntu).

---

## 🗂️ What you will build

A console project named **`CRUDWithMySQL`** backed by a MySQL database called `ProductDB`:

| File                       | Responsibility                                              |
| -------------------------- | ---------------------------------------------------------- |
| `Models/Product.cs`        | The `Product` entity (`Id`, `Name`, `Price`)               |
| `Data/ApplicationDbContext.cs` | The EF Core `DbContext` — exposes `DbSet<Product>` and the MySQL connection |
| `Program.cs`               | Runs Create → Read → Update → Delete against the database   |

**Flow:** `Product entity  →  ApplicationDbContext  →  migration  →  ProductDB table  →  CRUD in Program.cs`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (`code .`)
- A running **MySQL Server** instance on `localhost`
- NuGet packages: `Pomelo.EntityFrameworkCore.MySql`, `Microsoft.EntityFrameworkCore.Tools`
- The `dotnet-ef` CLI tool (installed locally via a tool manifest below)

---

## 🛠️ Steps

### Step 1 — Prepare the application

Create the console app, open it in VS Code, and add the EF Core packages and tooling.

```bash
# Scaffold the project and move into it
dotnet new console -n CRUDWithMySQL
cd CRUDWithMySQL

# Open the folder in VS Code
code .

# Add the MySQL provider and EF Core tools
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.2

# Install the dotnet-ef CLI as a local tool
dotnet new tool-manifest
dotnet tool install dotnet-ef
```

Create the database in MySQL:

```bash
# Open the MySQL CLI (enter the password when prompted)
mysql -u root -p
```

```sql
CREATE DATABASE ProductDB;
```

- Inside the project, create a folder named `Models` and a folder named `Data`.

### Step 2 — Define the data model

Create `Models/Product.cs` and define the `Product` entity: `Id` (primary key),
`Name`, and `Price`. EF Core treats a property named `Id` as the primary key by convention.

```csharp
namespace CRUDWithMySQL.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}
```

### Step 3 — Set up the database context

Create `Data/ApplicationDbContext.cs`. The context inherits from `DbContext`, exposes a
`DbSet<Product>` to manage the `Products` table, and configures the MySQL connection in
`OnConfiguring`.

```csharp
using Microsoft.EntityFrameworkCore;
using CRUDWithMySQL.Models;

namespace CRUDWithMySQL.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString =
            "Server=localhost;Database=ProductDB;User=efuser;Password=efpassword;";

        optionsBuilder.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 36))
        );
    }
}
```

> **Note:** The connection string above uses a dedicated `efuser`. Make sure that MySQL
> user exists and has privileges on `ProductDB`, or swap in credentials that do (for
> example `root` with your local password).

### Step 4 — Initialize the database

Create and apply a migration to generate the schema, then confirm the `Products` table
exists in `ProductDB`.

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 5 — Implement the CRUD operations

Replace the contents of `Program.cs` with the code below. It walks through every
operation in one run:

- **Create** — build a new `Product`, `Add` it to the `Products` `DbSet`, then `SaveChanges`.
- **Read** — list all products with `ToList`, and fetch one by key with `Find`.
- **Update** — `Find` the product, change `Price`, then `SaveChanges`.
- **Delete** — `Find` the product, `Remove` it, then `SaveChanges`.

```csharp
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

    // === READ (all) ===
    var products = context.Products.ToList();
    Console.WriteLine("\n📋 All products:");
    foreach (var p in products)
        Console.WriteLine($"{p.Id}: {p.Name} - {p.Price.ToString("C", mdl)}");

    // === READ (by id) ===
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
```

Run the program from the terminal:

```bash
dotnet run
```

---

## ▶️ Expected result

The console walks through the full lifecycle of a single `Laptop` product: it is created,
listed, found by `Id`, updated to a new price, and finally deleted — so the closing list
is empty again. Prices are formatted with the `ro-MD` culture.

```text
✅ Product created: Laptop

📋 All products:
1: Laptop - 1.200,50 L

🔍 Found product: Laptop - 1.200,50 L

✏️ Updated product: Laptop - 999,99 L

🗑 Deleted product: Laptop

📋 Final list:
```

---

## ☑️ Definition of done

- [ ] `CRUDWithMySQL` console project created with `Pomelo.EntityFrameworkCore.MySql` and `Microsoft.EntityFrameworkCore.Tools` added
- [ ] `dotnet-ef` installed via a local tool manifest
- [ ] `ProductDB` database created in MySQL
- [ ] `Models/Product.cs` defines the `Product` entity with `Id`, `Name`, `Price`
- [ ] `Data/ApplicationDbContext.cs` exposes `DbSet<Product>` and configures the MySQL connection
- [ ] `InitialCreate` migration applied with `dotnet ef database update`
- [ ] `Program.cs` performs Create, Read, Update, and Delete and `dotnet run` prints the lifecycle output

---

## 🔑 Key concepts

- **`DbContext` is the gateway** — it represents a session with the database; its `DbSet<T>`
  properties map entity classes to tables, and `SaveChanges` flushes tracked changes in a transaction.
- **Convention over configuration** — a property named `Id` becomes the primary key automatically,
  so the entity stays plain (a POCO) with no attributes required.
- **Migrations build and evolve the schema** — `dotnet ef migrations add` snapshots the model and
  `database update` applies it, keeping the database in step with your C# classes.
- **Change tracking drives CRUD** — `Add`, `Find` + property edits, and `Remove` simply mark
  entities; nothing hits the database until `SaveChanges` is called.
- **The provider is pluggable** — `UseMySql` (Pomelo) plus a matching `MySqlServerVersion` tells
  EF Core how to talk to MySQL; swapping providers is mostly a connection-and-package change.
