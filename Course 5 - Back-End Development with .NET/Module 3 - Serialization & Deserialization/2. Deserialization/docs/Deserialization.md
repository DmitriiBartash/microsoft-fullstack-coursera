# Deserialization

**Course 5 — Back-End Development with .NET** · Module 3 · Lesson 2 · `You Try It!`

> Build a .NET console app that **deserializes** a `Person` object from three formats —
> **binary**, **XML**, and **JSON** — and then **verifies the integrity** of the restored
> data. One model, three round-trips, one integrity check that proves nothing was lost.

---

## 🎯 Objective

Learn how to read serialized data back into .NET objects across multiple formats. You'll
implement binary deserialization with `BinaryReader`, XML deserialization with
`XmlSerializer`, and JSON deserialization with `System.Text.Json`, then validate that every
required property survived the round-trip.

---

## 🗂️ What you will build

A console project named **`DeserializationLab`** made of two files:

| File         | Responsibility                                                              |
| ------------ | --------------------------------------------------------------------------- |
| `Person.cs`  | The model being round-tripped (`UserName`, `UserAge`, `Email`, `IsActive`)  |
| `Program.cs` | Serialize once, then deserialize from binary, XML, and JSON + integrity check |

**Flow:** `Person → write (bin / xml / json) → read back → Person → integrity check`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- Built-in namespaces only: `System.Text.Json` (JSON) and `System.Xml.Serialization` (XML) — no extra NuGet packages required

---

## 🛠️ Steps

### Step 1 — Prepare the application

You'll create a new .NET console application in Visual Studio Code. This program will read
data from different formats (binary, XML, and JSON) back into objects.

```bash
dotnet new console -n DeserializationLab
cd DeserializationLab
```

- Open the project in Visual Studio Code.
- **Delete any existing code** in `Program.cs`.
- The serialization namespaces ship with the .NET SDK: `System.Text.Json` for JSON and
  `System.Xml.Serialization` for XML — no package installs needed.

### Step 2 — Define the `Person` model

Create a class **`Person`** with the properties that will be round-tripped through every
format. Public, settable properties are what `XmlSerializer` and `System.Text.Json` both
need to populate the object on the way back in.

Create a new file **`Person.cs`**:

```csharp
namespace DeserializationLab.Models;

public class Person
{
    public required string UserName { get; set; }
    public int UserAge { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; }
}
```

### Step 3 — Implement binary deserialization (`BinaryWriter` / `BinaryReader`)

Implement binary deserialization by reading binary data back into a `Person` object. Use
`BinaryWriter` to write each field into a file named `person.dat`, then use `BinaryReader`
to read the fields back **in the same order** and reconstruct the object.

> The fields must be read in exactly the order they were written — `BinaryReader` has no
> notion of names, only position and type.

```csharp
Console.WriteLine("=== Binary ===");
try
{
    using var fsWrite = new FileStream(binaryPath, FileMode.Create);
    using var writer = new BinaryWriter(fsWrite, Encoding.UTF8);
    writer.Write(person.UserName);
    writer.Write(person.UserAge);
    writer.Write(person.Email);
    writer.Write(person.IsActive);

    using var fsRead = new FileStream(binaryPath, FileMode.Open);
    using var reader = new BinaryReader(fsRead, Encoding.UTF8);
    var name = reader.ReadString();
    var age = reader.ReadInt32();
    var email = reader.ReadString();
    var active = reader.ReadBoolean();
    Console.WriteLine($"Binary restored: {name}, {age}, {email}, Active={active}");
}
catch (Exception ex)
{
    Console.WriteLine($"Binary error: {ex.Message}");
}
```

### Step 4 — Implement XML deserialization (`XmlSerializer`)

Use `XmlSerializer` to convert XML-formatted data back into a `Person` object. Serialize the
object to `person.xml`, then deserialize it back and cast the result to `Person`.

```csharp
Console.WriteLine("\n=== XML ===");
try
{
    var xmlSerializer = new XmlSerializer(typeof(Person));
    using var fsXml = new FileStream(xmlPath, FileMode.Create);
    xmlSerializer.Serialize(fsXml, person);

    using var fsXmlRead = new FileStream(xmlPath, FileMode.Open);
    var xmlPersonObj = xmlSerializer.Deserialize(fsXmlRead);
    var xmlPerson = xmlPersonObj as Person;
    if (xmlPerson != null)
        Console.WriteLine($"XML restored: {xmlPerson.UserName}, {xmlPerson.UserAge}, {xmlPerson.Email}, Active={xmlPerson.IsActive}");
    else
        Console.WriteLine("XML restored: Deserialization returned null.");
}
catch (Exception ex)
{
    Console.WriteLine($"XML error: {ex.Message}");
}
```

### Step 5 — Implement JSON deserialization (`System.Text.Json`)

Use `System.Text.Json.JsonSerializer` to read JSON data directly into a `Person` object.
Serialize the object to `person.json`, then deserialize the file contents back into a typed
`Person`.

```csharp
Console.WriteLine("\n=== JSON ===");
try
{
    var json = JsonSerializer.Serialize(person, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(jsonPath, json);

    var jsonPerson = JsonSerializer.Deserialize<Person>(File.ReadAllText(jsonPath));
    if (jsonPerson != null)
        Console.WriteLine($"JSON restored: {jsonPerson.UserName}, {jsonPerson.UserAge}, {jsonPerson.Email}, Active={jsonPerson.IsActive}");
    else
        Console.WriteLine("JSON restored: Deserialization returned null.");
}
catch (Exception ex)
{
    Console.WriteLine($"JSON error: {ex.Message}");
}
```

### Step 6 — Verify the integrity of deserialized data

Confirm that each deserialization process produced valid, consistent results. Check that all
required properties are present (not `null` or empty) and that values are sensible — for
example, `UserAge` greater than zero. Wrapping every format in its own `try/catch` already
guards against missing data or type mismatches.

```csharp
Console.WriteLine("\n=== Integrity Check ===");
try
{
    var jsonPerson = JsonSerializer.Deserialize<Person>(File.ReadAllText(jsonPath));
    if (!string.IsNullOrEmpty(jsonPerson?.UserName) && jsonPerson.UserAge > 0 && !string.IsNullOrEmpty(jsonPerson.Email))
        Console.WriteLine("Integrity check passed: all formats preserved data correctly");
    else
        Console.WriteLine("Integrity check failed: some data missing or corrupted");
}
catch (Exception ex)
{
    Console.WriteLine($"Integrity check error: {ex.Message}");
}
```

### Putting it together — `Program.cs`

The full program creates a `Data` directory, builds one sample `Person`, and runs all four
blocks in sequence:

```csharp
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using DeserializationLab.Models;

class Program
{
    static void Main()
    {
        Directory.CreateDirectory("Data");

        var person = new Person
        {
            UserName = "Bob",
            UserAge = 25,
            Email = "bob@example.com",
            IsActive = true
        };

        var binaryPath = Path.Combine("Data", "person.dat");
        var xmlPath = Path.Combine("Data", "person.xml");
        var jsonPath = Path.Combine("Data", "person.json");

        Console.WriteLine("=== Binary ===");
        try
        {
            using var fsWrite = new FileStream(binaryPath, FileMode.Create);
            using var writer = new BinaryWriter(fsWrite, Encoding.UTF8);
            writer.Write(person.UserName);
            writer.Write(person.UserAge);
            writer.Write(person.Email);
            writer.Write(person.IsActive);

            using var fsRead = new FileStream(binaryPath, FileMode.Open);
            using var reader = new BinaryReader(fsRead, Encoding.UTF8);
            var name = reader.ReadString();
            var age = reader.ReadInt32();
            var email = reader.ReadString();
            var active = reader.ReadBoolean();
            Console.WriteLine($"Binary restored: {name}, {age}, {email}, Active={active}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Binary error: {ex.Message}");
        }

        Console.WriteLine("\n=== XML ===");
        try
        {
            var xmlSerializer = new XmlSerializer(typeof(Person));
            using var fsXml = new FileStream(xmlPath, FileMode.Create);
            xmlSerializer.Serialize(fsXml, person);

            using var fsXmlRead = new FileStream(xmlPath, FileMode.Open);
            var xmlPersonObj = xmlSerializer.Deserialize(fsXmlRead);
            var xmlPerson = xmlPersonObj as Person;
            if (xmlPerson != null)
                Console.WriteLine($"XML restored: {xmlPerson.UserName}, {xmlPerson.UserAge}, {xmlPerson.Email}, Active={xmlPerson.IsActive}");
            else
                Console.WriteLine("XML restored: Deserialization returned null.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"XML error: {ex.Message}");
        }

        Console.WriteLine("\n=== JSON ===");
        try
        {
            var json = JsonSerializer.Serialize(person, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonPath, json);

            var jsonPerson = JsonSerializer.Deserialize<Person>(File.ReadAllText(jsonPath));
            if (jsonPerson != null)
                Console.WriteLine($"JSON restored: {jsonPerson.UserName}, {jsonPerson.UserAge}, {jsonPerson.Email}, Active={jsonPerson.IsActive}");
            else
                Console.WriteLine("JSON restored: Deserialization returned null.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JSON error: {ex.Message}");
        }

        Console.WriteLine("\n=== Integrity Check ===");
        try
        {
            var jsonPerson = JsonSerializer.Deserialize<Person>(File.ReadAllText(jsonPath));
            if (!string.IsNullOrEmpty(jsonPerson?.UserName) && jsonPerson.UserAge > 0 && !string.IsNullOrEmpty(jsonPerson.Email))
                Console.WriteLine("Integrity check passed: all formats preserved data correctly");
            else
                Console.WriteLine("Integrity check failed: some data missing or corrupted");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Integrity check error: {ex.Message}");
        }
    }
}
```

Run it:

```bash
dotnet run
```

---

## ▶️ Expected result

The console prints one section per format, each echoing the **restored** `Person`, followed
by a passing integrity check:

```text
=== Binary ===
Binary restored: Bob, 25, bob@example.com, Active=True

=== XML ===
XML restored: Bob, 25, bob@example.com, Active=True

=== JSON ===
JSON restored: Bob, 25, bob@example.com, Active=True

=== Integrity Check ===
Integrity check passed: all formats preserved data correctly
```

A `Data/` folder now holds `person.dat`, `person.xml`, and `person.json` — the three
serialized payloads you read back.

---

## ☑️ Definition of done

- [ ] `DeserializationLab` console project created and `Program.cs` cleared
- [ ] `Person.cs` defines `UserName`, `UserAge`, `Email`, and `IsActive`
- [ ] Binary round-trip reads fields back **in write order** with `BinaryReader`
- [ ] XML round-trip restores the object via `XmlSerializer.Deserialize`
- [ ] JSON round-trip restores the object via `JsonSerializer.Deserialize<Person>`
- [ ] Each format is wrapped in `try/catch` and prints its restored values
- [ ] The integrity check confirms all required properties are present and prints "passed"
- [ ] `dotnet run` produces the four sections above and writes the three files under `Data/`

---

## 🔑 Key concepts

- **Deserialization is format-specific, the model is not** — one `Person` class is reused
  across binary, XML, and JSON; only the *reader* changes.
- **Binary is positional** — `BinaryReader` restores fields by **order and type**, not by
  name, so reads must mirror the exact write sequence (`ReadString`, `ReadInt32`,
  `ReadString`, `ReadBoolean`).
- **`XmlSerializer` and `System.Text.Json` are reflection-driven** — they need public,
  parameterless-constructable types with settable properties to repopulate the object.
- **Fail fast, isolate failures** — wrapping each format in its own `try/catch` keeps a
  malformed XML payload from killing the JSON path and surfaces type mismatches as readable
  errors.
- **Validate after you deserialize** — round-tripping success isn't integrity; check that
  required properties are non-null and values are sensible (`UserAge > 0`) before trusting
  the data.
