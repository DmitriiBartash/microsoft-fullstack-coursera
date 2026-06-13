# Implementation of .NET Libraries

**Course 5 — Back-End Development with .NET** · Module 1 · `You Try It!`

> Build a C# console app that integrates real .NET libraries through **NuGet** and
> exercises them end-to-end. You parse and serialize JSON two ways — once with the
> built-in **`System.Text.Json`**, once with the popular **`Newtonsoft.Json`** package —
> and trace the run with **`Serilog`**, so you can see exactly what NuGet buys you.

---

## 🎯 Objective

By the end of this activity you will be able to **integrate commonly used .NET libraries
into a C# project using NuGet** and demonstrate their basic functionality: parsing a JSON
string into a typed object, accessing values dynamically, and serializing an object back to
JSON — comparing the built-in serializer against an external package.

---

## 🗂️ What you will build

A console project named **`JsonExample`** that loads three libraries and runs two parallel
JSON variants in `Program.cs`:

| Library              | Source            | What it does in this lab                                  |
| -------------------- | ----------------- | --------------------------------------------------------- |
| `System.Text.Json`   | Built-in (BCL)    | Variant A — dynamic-like parse, typed deserialize, serialize |
| `Newtonsoft.Json`    | NuGet package     | Variant B — `dynamic` parse, typed deserialize, serialize |
| `Serilog`            | NuGet package     | Structured logging of each variant's completion           |

**Flow:** `JSON string  →  Parse / Deserialize  →  Person object  →  Serialize  →  JSON string` — run for both Variant A and Variant B.

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code with the C# extension
- A terminal (VS Code: **View > Terminal**)
- NuGet packages added in the steps below: `Newtonsoft.Json`, `Serilog`, `Serilog.Sinks.Console`

---

## 🛠️ Steps

### Step 1 — Create a basic C# console application in VS Code

Before working with any .NET libraries, scaffold a console project.

```bash
mkdir MyFirstConsoleApp
cd MyFirstConsoleApp
dotnet new console -n JsonExample
cd JsonExample
code .
```

- `dotnet new console -n JsonExample` creates a folder named `JsonExample` with the files for a basic console app.
- Confirm the project exists by checking for `Program.cs` — it contains a basic "Hello World" program.

### Step 2 — Run the basic console application

Make sure everything is wired up before adding libraries.

- Ensure you are in the project root (where `Program.cs` and `JsonExample.csproj` live).
- Build and run it:

```bash
dotnet run
```

- Confirm the output displays `Hello World!`.

### Step 3 — Install .NET libraries using NuGet

Add `Newtonsoft.Json` (widely used for handling JSON in web apps and APIs) plus `Serilog`
for logging. From the project folder:

```bash
dotnet add package Newtonsoft.Json
dotnet add package Serilog
dotnet add package Serilog.Sinks.Console
```

- Verify the packages were added by checking `JsonExample.csproj` — each should appear as a `<PackageReference>`:

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  <PackageReference Include="Serilog" Version="4.0.0" />
  <PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
</ItemGroup>
```

> `System.Text.Json` needs no package — it ships with the .NET runtime, so it is available by default.

### Step 4 — Parse JSON with the .NET libraries

Open `Program.cs`, replace its contents, and define a `Person` model plus a `Main` that
**parses** JSON into objects. Variant A uses `System.Text.Json`; Variant B uses
`Newtonsoft.Json`. Both also show dynamic-like access to a single property.

- Define a class named `Person` with `Name`, `Age`, and `Country`.
- Create JSON strings inside `Main`.
- Use each library to convert a JSON string into a `Person` object and print the result.

### Step 5 — Serialize an object back to JSON

In the same `Main` method, take a `Person` object and convert it **back** into a JSON string
with each library, then run the program and verify the JSON output.

- Build a new `Person` and serialize it with `System.Text.Json.JsonSerializer.Serialize(...)` (Variant A).
- Build another `Person` and serialize it with `Newtonsoft.Json.JsonConvert.SerializeObject(...)` (Variant B).
- Log completion of each variant with `Serilog`, then run:

```bash
dotnet run
```

The complete `Program.cs`:

```csharp
using System.Text.Json;
using Newtonsoft.Json;
using Serilog;

class Person
{
    public required string Name { get; set; }
    public int Age { get; set; }
    public required string Country { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        Console.WriteLine("=== Variant A: System.Text.Json ===");

        string jsonA1 = @"{ ""Name"": ""Alice"", ""Age"": 30, ""Country"": ""USA"" }";
        using var doc = JsonDocument.Parse(jsonA1);
        var root = doc.RootElement;
        Console.WriteLine("Dynamic-like name: " + root.GetProperty("Name").GetString());

        string jsonA2 = @"{ ""Name"": ""Bob"", ""Age"": 25, ""Country"": ""UK"" }";
        var typedA = System.Text.Json.JsonSerializer.Deserialize<Person>(jsonA2);
        Console.WriteLine("Typed name: " + (typedA != null ? typedA.Name : "null"));

        var newPersonA = new Person { Name = "Diana", Age = 28, Country = "Germany" };
        string serializedA = System.Text.Json.JsonSerializer.Serialize(
            newPersonA, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("Serialized JSON (A):");
        Console.WriteLine(serializedA);

        Console.WriteLine("\nA notes:");
        Console.WriteLine("- Built-in, no extra dependency.");
        Console.WriteLine("- Dynamic-like access uses JsonDocument and explicit property APIs.");
        Console.WriteLine("- Strong typing via Person works similarly to B.");
        Console.WriteLine("- Fast and modern defaults; fewer convenience features for dynamic work.");
        Log.Information("Completed Variant A");

        Console.WriteLine("\n=== Variant B: Newtonsoft.Json ===");

        string jsonB1 = @"{ ""Name"": ""Eve"", ""Age"": 32, ""Country"": ""Canada"" }";
        var dynB = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonB1);
        Console.WriteLine("Dynamic name: " + (dynB != null ? dynB.Name.ToString() : "null"));

        string jsonB2 = @"{ ""Name"": ""Frank"", ""Age"": 27, ""Country"": ""France"" }";
        var typedB = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(jsonB2);
        Console.WriteLine("Typed name: " + (typedB != null ? typedB.Name : "null"));

        var newPersonB = new Person { Name = "Grace", Age = 29, Country = "Italy" };
        string serializedB = Newtonsoft.Json.JsonConvert.SerializeObject(newPersonB, Formatting.Indented);
        Console.WriteLine("Serialized JSON (B):");
        Console.WriteLine(serializedB);

        Console.WriteLine("\nB notes:");
        Console.WriteLine("- External package via NuGet.");
        Console.WriteLine("- Dynamic parsing is straightforward with dynamic/JObject.");
        Console.WriteLine("- Rich feature set and ecosystem; great for complex scenarios.");
        Console.WriteLine("- Adds a dependency but very popular and flexible.");

        Console.WriteLine("\nKey differences:");
        Console.WriteLine("- A is dependency-free and uses structured JsonDocument for dynamic-like access.");
        Console.WriteLine("- B uses a NuGet package and offers easier dynamic handling and more features.");
        Console.WriteLine("- Both support strongly-typed models; outputs are similar for basic cases.");
        Log.Information("Completed Variant B");
    }
}
```

> Both `using System.Text.Json;` and `using Newtonsoft.Json;` are in scope, so the code uses
> fully-qualified `System.Text.Json.JsonSerializer` and `Newtonsoft.Json.JsonConvert` to keep
> the two variants unambiguous. `Formatting.Indented` resolves to `Newtonsoft.Json.Formatting`.

---

## ▶️ Expected result

Running `dotnet run` prints both variants. Each shows the dynamic-like name, the typed name,
and the indented serialized JSON, followed by Serilog's `Completed Variant A` / `Completed Variant B`
information lines:

```text
=== Variant A: System.Text.Json ===
Dynamic-like name: Alice
Typed name: Bob
Serialized JSON (A):
{
  "Name": "Diana",
  "Age": 28,
  "Country": "Germany"
}
...
[12:00:00 INF] Completed Variant A

=== Variant B: Newtonsoft.Json ===
Dynamic name: Eve
Typed name: Frank
Serialized JSON (B):
{
  "Name": "Grace",
  "Age": 29,
  "Country": "Italy"
}
...
[12:00:00 INF] Completed Variant B
```

---

## ☑️ Definition of done

- [ ] `JsonExample` console project created with `dotnet new console` and runs `Hello World!`
- [ ] `Newtonsoft.Json`, `Serilog`, and `Serilog.Sinks.Console` added via `dotnet add package` and visible in `JsonExample.csproj`
- [ ] `Person` class defined with `Name`, `Age`, and `Country`
- [ ] Variant A parses, typed-deserializes, and serializes JSON with `System.Text.Json`
- [ ] Variant B does the same with `Newtonsoft.Json`, including `dynamic` access
- [ ] `Serilog` logs `Completed Variant A` and `Completed Variant B`
- [ ] `dotnet run` prints both variants' parsed names and indented JSON output

---

## 🔑 Key concepts

- **NuGet integrates libraries** — `dotnet add package` downloads a dependency and records it
  as a `<PackageReference>` in the `.csproj`; restore happens automatically on build.
- **Built-in vs. external** — `System.Text.Json` ships with .NET (no package, fast, modern
  defaults), while `Newtonsoft.Json` is an external package with richer, more flexible features.
- **Typed vs. dynamic parsing** — both libraries can deserialize into a strongly-typed
  `Person` *or* expose values dynamically (`JsonDocument`/`GetProperty` for A, `dynamic`/`JObject` for B).
- **Serialization is the inverse** — `JsonSerializer.Serialize` and `JsonConvert.SerializeObject`
  turn an object back into JSON; `WriteIndented` / `Formatting.Indented` make the output readable.
- **Structured logging with Serilog** — configure a `LoggerConfiguration` once with a sink
  (`WriteTo.Console()`), then emit events like `Log.Information(...)` to trace program flow.
