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
