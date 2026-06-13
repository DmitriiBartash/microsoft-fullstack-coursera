# Implementing Serialization in .NET

**Course 5 — Back-End Development with .NET** · Module 3 · Lesson 1 · `You Try It!`

> Build a small .NET console app that serializes a list of `Person` objects three
> ways — **binary**, **XML**, and **JSON** — then **deserializes** each back and
> **compares** the resulting file sizes and readability. Each format lives in its own
> single-responsibility helper class, and `Program.cs` just wires them together.

---

## 🎯 Objective

By the end of this lab you will set up a .NET project for serialization and implement
**binary**, **XML**, and **JSON** serialization (plus their round-trip deserialization).
You will then compare the output of each method to understand its use cases and
performance trade-offs.

---

## 🗂️ What you will build

A console project named **`SerializationDemo`** organized into focused files:

| File                       | Responsibility                                            |
| -------------------------- | -------------------------------------------------------- |
| `Models/Person.cs`         | The data model — `UserName` and `UserAge`                |
| `Serializers/BinarySerializerManual.cs` | Write/read each field manually with `BinaryWriter`/`BinaryReader` |
| `Serializers/XmlSerializerHelper.cs`    | Serialize/deserialize via `XmlSerializer`   |
| `Serializers/JsonSerializerHelper.cs`   | Serialize/deserialize via `System.Text.Json`|
| `Program.cs`               | Orchestrate: generate data, write all three formats, compare, read back |

**Flow:** `Program  →  GeneratePeople()  →  Binary / XML / JSON Serialize  →  files  →  Deserialize  →  compare`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (with the C# extension)
- No extra NuGet packages — binary, XML, and JSON serialization all ship with the .NET base class library

---

## 🛠️ Steps

### Step 1 — Set up the .NET project for serialization

In the Visual Studio Code terminal, navigate to the folder where you want the project,
then scaffold a new console app and move into it.

```bash
dotnet new console -o SerializationDemo
cd SerializationDemo
```

Create the model. Add a `Models` folder with a `Person` class exposing `UserName` and
`UserAge`.

```csharp
// Models/Person.cs
namespace SerializationDemo.Models;

public class Person
{
    public string UserName { get; set; } = string.Empty;
    public int UserAge { get; set; }
}
```

### Step 2 — Implement binary serialization

Write each property manually with a `BinaryWriter` over a `FileStream`, and read it back
in the same order with a `BinaryReader`. The leading `Count` lets the reader know how many
records to expect.

```csharp
// Serializers/BinarySerializerManual.cs
using SerializationDemo.Models;

namespace SerializationDemo.Serializers;

public static class BinarySerializerManual
{
    public static void Serialize(List<Person> people, string filePath)
    {
        using FileStream fs = new FileStream(filePath, FileMode.Create);
        using BinaryWriter writer = new BinaryWriter(fs);

        writer.Write(people.Count);
        foreach (var person in people)
        {
            writer.Write(person.UserName);
            writer.Write(person.UserAge);
        }
    }

    public static List<Person> Deserialize(string filePath)
    {
        using FileStream fs = new FileStream(filePath, FileMode.Open);
        using BinaryReader reader = new BinaryReader(fs);

        int count = reader.ReadInt32();
        var people = new List<Person>(count);
        for (int i = 0; i < count; i++)
        {
            string? name = reader.ReadString();
            int age = reader.ReadInt32();
            if (name == null)
                throw new InvalidOperationException("Binary deserialization failed: name is null.");
            people.Add(new Person { UserName = name, UserAge = age });
        }
        return people;
    }
}
```

> **Note:** `BinaryFormatter` is obsolete and disabled by default in modern .NET for
> security reasons. Writing fields explicitly with `BinaryWriter`/`BinaryReader` is the
> safe, supported way to produce a compact binary file.

### Step 3 — Implement XML serialization

Use `XmlSerializer` over the `List<Person>` type. It writes a human-readable XML document
and reconstructs the list on read.

```csharp
// Serializers/XmlSerializerHelper.cs
using System.Xml.Serialization;
using SerializationDemo.Models;

namespace SerializationDemo.Serializers;

public static class XmlSerializerHelper
{
    public static void Serialize(List<Person> people, string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Person>));
        using FileStream fs = new FileStream(filePath, FileMode.Create);
        serializer.Serialize(fs, people);
    }

    public static List<Person> Deserialize(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Person>));
        using FileStream fs = new FileStream(filePath, FileMode.Open);
        var obj = serializer.Deserialize(fs);
        if (obj is List<Person> people)
            return people;
        throw new InvalidOperationException("XML deserialization failed: object is null or not List<Person>.");
    }
}
```

### Step 4 — Implement JSON serialization

Use the built-in `System.Text.Json`. `WriteIndented = true` produces pretty-printed JSON
so the file is easy to read.

```csharp
// Serializers/JsonSerializerHelper.cs
using System.Text.Json;
using SerializationDemo.Models;

namespace SerializationDemo.Serializers;

public static class JsonSerializerHelper
{
    public static void Serialize(List<Person> people, string filePath)
    {
        var json = JsonSerializer.Serialize(people, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public static List<Person> Deserialize(string filePath)
    {
        string json = File.ReadAllText(filePath);
        var people = JsonSerializer.Deserialize<List<Person>>(json);
        if (people is not null)
            return people;
        throw new InvalidOperationException("JSON deserialization failed: object is null.");
    }
}
```

### Step 5 — Wire it together and compare the outputs

In `Program.cs`, generate sample data, write all three formats into a `Data` folder, print
each file size, then deserialize each format back and confirm the round-trip restored the
data.

```csharp
// Program.cs
using SerializationDemo.Models;
using SerializationDemo.Serializers;

namespace SerializationDemo;

class Program
{
    static void Main()
    {
        string dataDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        Directory.CreateDirectory(dataDir);

        var people = GeneratePeople(20);

        string binPath = Path.Combine(dataDir, "people.dat");
        BinarySerializerManual.Serialize(people, binPath);
        Console.WriteLine($"Binary serialization complete: {binPath}");

        string xmlPath = Path.Combine(dataDir, "people.xml");
        XmlSerializerHelper.Serialize(people, xmlPath);
        Console.WriteLine($"XML serialization complete: {xmlPath}");

        string jsonPath = Path.Combine(dataDir, "people.json");
        JsonSerializerHelper.Serialize(people, jsonPath);
        Console.WriteLine($"JSON serialization complete: {jsonPath}");

        Console.WriteLine("\n--- Compare Outputs ---");
        Console.WriteLine($"Binary file size: {new FileInfo(binPath).Length} bytes");
        Console.WriteLine($"XML file size: {new FileInfo(xmlPath).Length} bytes");
        Console.WriteLine($"JSON file size: {new FileInfo(jsonPath).Length} bytes");

        Console.WriteLine("\n--- Deserialization Results ---");
        var binPeople = BinarySerializerManual.Deserialize(binPath);
        Console.WriteLine($"Binary restored count: {binPeople.Count}, first: {binPeople[0].UserName} ({binPeople[0].UserAge})");

        var xmlPeople = XmlSerializerHelper.Deserialize(xmlPath);
        Console.WriteLine($"XML restored count: {xmlPeople.Count}, first: {xmlPeople[0].UserName} ({xmlPeople[0].UserAge})");

        var jsonPeople = JsonSerializerHelper.Deserialize(jsonPath);
        Console.WriteLine($"JSON restored count: {jsonPeople.Count}, first: {jsonPeople[0].UserName} ({jsonPeople[0].UserAge})");
    }

    static List<Person> GeneratePeople(int count)
    {
        var rnd = new Random();
        var list = new List<Person>();
        for (int i = 1; i <= count; i++)
        {
            list.Add(new Person
            {
                UserName = $"User{i}",
                UserAge = rnd.Next(18, 70)
            });
        }
        return list;
    }
}
```

Run it:

```bash
dotnet run
```

---

## ▶️ Expected result

The console prints three "serialization complete" lines, then a size comparison and the
deserialization results. The **binary** file is the smallest and least readable, **XML**
is the largest and most verbose, and **JSON** sits in between while staying human-readable.
Open the generated `Data/people.dat`, `Data/people.xml`, and `Data/people.json` to see the
difference for yourself.

```text
Binary serialization complete: .../Data/people.dat
XML serialization complete: .../Data/people.xml
JSON serialization complete: .../Data/people.json

--- Compare Outputs ---
Binary file size: 213 bytes
XML file size: 1024 bytes
JSON file size: 612 bytes

--- Deserialization Results ---
Binary restored count: 20, first: User1 (34)
XML restored count: 20, first: User1 (34)
JSON restored count: 20, first: User1 (34)
```

*(Exact byte counts and ages vary because the sample data is randomized.)*

---

## ☑️ Definition of done

- [ ] `SerializationDemo` console project created with a `Person` model (`UserName`, `UserAge`)
- [ ] `BinarySerializerManual` writes and reads each field with `BinaryWriter`/`BinaryReader`
- [ ] `XmlSerializerHelper` serializes and deserializes via `XmlSerializer`
- [ ] `JsonSerializerHelper` serializes and deserializes via `System.Text.Json`
- [ ] `Program.cs` writes all three files, prints their sizes, and reads each back
- [ ] `dotnet run` confirms every format restores the full list of 20 people

---

## 🔑 Key concepts

- **One format, one class** — each serializer is a small static helper with a single reason
  to change, so swapping or testing a format never touches the others.
- **Binary is compact but rigid** — `BinaryWriter`/`BinaryReader` give the smallest file, but
  reader and writer must agree on field order exactly; there is no self-describing schema.
- **XML is verbose but self-describing** — `XmlSerializer` produces the largest, most
  human- and tooling-friendly output, ideal for interoperability and config.
- **JSON is the balanced default** — `System.Text.Json` is compact, readable, and the de
  facto wire format for web APIs; `WriteIndented` trades a few bytes for readability.
- **Round-trip is the real test** — serialization only counts if `Deserialize(Serialize(x))`
  reconstructs the original data, which is why each helper validates and reports its result.
