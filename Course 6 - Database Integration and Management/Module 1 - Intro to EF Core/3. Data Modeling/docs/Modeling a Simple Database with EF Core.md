# Modeling a Simple Database with EF Core

**Course 6 — Database Integration and Management** · Module 1 · Lesson 3 · `You Try It!`

> Build a small .NET console app that models a simple HR database with **Entity
> Framework Core** and **SQLite**. You will define `Employee` and `Department`
> entities, wire a **one-to-many** relationship in a `DbContext`, seed data,
> generate the schema with **migrations**, and run **CRUD** queries end-to-end.

---

## 🎯 Objective

By the end of this activity, you will be able to apply data modeling techniques to
create a simple database using **EF Core** in a .NET application — modeling entities,
configuring relationships, seeding data, and querying with LINQ.

---

## 🗂️ What you will build

A console project named **`EFCoreModelApp`** backed by a SQLite database (`hr.db`):

| File                       | Responsibility                                                        |
| -------------------------- | --------------------------------------------------------------------- |
| `Models/Department.cs`     | `Department` entity — one department has many employees               |
| `Models/Employee.cs`       | `Employee` entity — belongs to one department (navigation property)   |
| `Data/HRDbContext.cs`      | `DbContext` — `DbSet`s, SQLite connection, relationship, seed data    |
| `Program.cs`               | Create the DB, run LINQ queries, insert a new employee                |

**Flow:** `Models  →  HRDbContext (DbSets + relationship + seed)  →  migration  →  hr.db  →  LINQ queries`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- A code editor (Visual Studio Code or Visual Studio)
- The EF Core CLI tools (`dotnet-ef`) — installed in Step 1

---

## 🛠️ Steps

### Step 1 — Install required tools and create a new console application

Set up your environment, install the necessary dependencies, and create a new console
application for your EF Core project.

```bash
# Install the EF Core command-line tools globally
dotnet tool install --global dotnet-ef

# Create the console app and move into the project directory
dotnet new console -n EFCoreModelApp
cd EFCoreModelApp

# Add the SQLite provider and the design-time package used by migrations
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design

# Confirm the project builds and runs
dotnet run
```

### Step 2 — Create and configure entity classes

Create a `Models` folder and define the `Employee` and `Department` entity classes that
represent the tables in your database. Note that `Employee` carries both the
`DepartmentId` **foreign key** and a `Department` **navigation property** for its
associated department.

**`Models/Department.cs`:**

```csharp
namespace EFCoreModelApp.Models;

public class Department
{
    public int DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Employee> Employees { get; set; } = [];
}
```

**`Models/Employee.cs`:**

```csharp
namespace EFCoreModelApp.Models;

public class Employee
{
    public int EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
}
```

### Step 3 — Set up the DbContext

Create the `HRDbContext` class to manage the database connection and the relationships
between entities. It exposes a `DbSet` for each entity, points to a SQLite file in
`OnConfiguring`, configures the one-to-many relationship in `OnModelCreating`, and seeds
initial departments and employees.

**`Data/HRDbContext.cs`:**

```csharp
using Microsoft.EntityFrameworkCore;
using EFCoreModelApp.Models;

namespace EFCoreModelApp.Data;

public class HRDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=hr.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One-to-many relationship
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId);

        // Seed initial data
        modelBuilder.Entity<Department>().HasData(
            new Department { DepartmentId = 1, Name = "HR" },
            new Department { DepartmentId = 2, Name = "IT" }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee { EmployeeId = 1, Name = "Alice", Position = "HR Manager", DepartmentId = 1 },
            new Employee { EmployeeId = 2, Name = "Bob", Position = "Developer", DepartmentId = 2 }
        );
    }
}
```

### Step 4 — Add and apply migrations

Generate the database schema from your model and apply it to the SQLite database. The
first migration captures the current model state (entities, relationship, and seed data);
applying it creates `hr.db` with the `Employees` and `Departments` tables.

```bash
# Capture the current model state as a migration
dotnet ef migrations add InitialCreate

# Create the database and tables from the migration
dotnet ef database update
```

### Step 5 — Test the application

Replace `Program.cs` with a program that exercises **CRUD** operations: list all employees
(with their department names), filter to the HR department, insert a new employee if one
does not already exist, then re-display the data. The helper `PrintTable` reflects over an
object's properties so any projection can be printed as a tidy table.

**`Program.cs`:**

```csharp
using EFCoreModelApp.Data;
using EFCoreModelApp.Models;

using var context = new HRDbContext();
context.Database.EnsureCreated();

void PrintTable<T>(IEnumerable<T> items, string title)
{
    Console.WriteLine($"\n{title}");
    Console.WriteLine(new string('-', 100));
    var props = typeof(T).GetProperties();
    foreach (var prop in props)
        Console.Write($"{prop.Name,-30}");
    Console.WriteLine();
    Console.WriteLine(new string('-', 100));
    foreach (var item in items)
    {
        foreach (var prop in props)
        {
            var value = prop.GetValue(item) ?? "";
            Console.Write($"{value,-30}");
        }
        Console.WriteLine();
    }
    Console.WriteLine(new string('-', 100));
}

var employees = context.Employees
    .Select(e => new { e.EmployeeId, e.Name, e.Position, Department = e.Department.Name })
    .ToList();
PrintTable(employees, "All Employees");

var hrEmployees = context.Employees
    .Where(e => e.Department.Name == "HR")
    .Select(e => new { e.EmployeeId, e.Name, e.Position, Department = e.Department.Name })
    .ToList();
PrintTable(hrEmployees, "Employees in HR Department");

if (!context.Employees.Any(e => e.Name == "Charlie"))
{
    var newEmployee = new Employee
    {
        Name = "Charlie",
        Position = "System Administrator",
        DepartmentId = 2
    };
    context.Employees.Add(newEmployee);
    context.SaveChanges();
    Console.WriteLine("\nAdded new employee: Charlie (System Administrator, IT).");
}
else
{
    Console.WriteLine("\nCharlie already exists. Skipping insert.");
}

var updatedEmployees = context.Employees
    .Select(e => new { e.EmployeeId, e.Name, e.Position, Department = e.Department.Name })
    .ToList();
PrintTable(updatedEmployees, "Updated Employees List");

var departments = context.Departments
    .Select(d => new { d.DepartmentId, d.Name })
    .ToList();
PrintTable(departments, "Departments");
```

Run the application to verify the database operations work correctly:

```bash
dotnet run
```

---

## ▶️ Expected result

The app prints four tables: **All Employees** (Alice and Bob with their department names),
**Employees in HR Department** (just Alice), an **Updated Employees List** that now
includes **Charlie** in IT, and the **Departments** table (HR and IT). On the first run it
reports `Added new employee: Charlie (System Administrator, IT).`; on later runs it reports
`Charlie already exists. Skipping insert.`

---

## ☑️ Definition of done

- [ ] `dotnet-ef` tools installed and the `EFCoreModelApp` console project created
- [ ] `Microsoft.EntityFrameworkCore.Sqlite` and `...Design` packages added
- [ ] `Department` and `Employee` entities defined, with `Employee` holding the `Department` navigation property
- [ ] `HRDbContext` exposes both `DbSet`s, the SQLite connection, the one-to-many relationship, and seed data
- [ ] `dotnet ef migrations add` and `dotnet ef database update` create `hr.db` with both tables
- [ ] `dotnet run` lists employees with departments, filters HR, and inserts Charlie

---

## 🔑 Key concepts

- **Entities map to tables** — each plain C# class (`Employee`, `Department`) becomes a table, and its properties become columns; the `Id`-suffixed property is the convention-based primary key.
- **One-to-many via navigation + FK** — `HasOne().WithMany().HasForeignKey()` models the relationship; `Employee.DepartmentId` is the foreign key and `Employee.Department` / `Department.Employees` are the navigation properties that let LINQ traverse it.
- **Seeding with `HasData`** — seed data lives in the model, so it is baked into the migration and inserted deterministically when the schema is created.
- **Migrations are the schema source of truth** — `migrations add` records the model state and `database update` applies it, keeping the database in sync with the code as the model evolves.
- **LINQ to SQL translation** — `Where`/`Select` projections (e.g. filtering by `e.Department.Name`) are translated to SQL and run in the database, so you query objects without writing SQL by hand.
