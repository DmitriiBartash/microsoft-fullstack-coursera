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
        string serializedA = System.Text.Json.JsonSerializer.Serialize(newPersonA, new JsonSerializerOptions { WriteIndented = true });
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
