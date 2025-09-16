﻿using SerializationDemo.Models;
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
