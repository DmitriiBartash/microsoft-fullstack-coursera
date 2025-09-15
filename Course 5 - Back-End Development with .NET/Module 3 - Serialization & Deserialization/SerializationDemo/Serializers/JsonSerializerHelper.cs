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
