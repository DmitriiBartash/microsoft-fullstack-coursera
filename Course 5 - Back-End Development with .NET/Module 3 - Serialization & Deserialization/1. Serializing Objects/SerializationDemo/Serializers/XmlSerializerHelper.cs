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
