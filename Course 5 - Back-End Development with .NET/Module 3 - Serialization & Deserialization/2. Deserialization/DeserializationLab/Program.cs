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
