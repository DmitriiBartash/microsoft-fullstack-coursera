using SerializationSecurityApp.Models;
using SerializationSecurityApp.Services;

class Program
{
    static void Main()
    {
        var serializer = new SerializationService();
        string? jsonData = null;

        while (jsonData == null)
        {
            Console.WriteLine("\n=== User Registration ===");
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Enter Email: ");
            string email = Console.ReadLine() ?? "";

            Console.Write("Enter Password: ");
            string password = Console.ReadLine() ?? "";

            var user = new User
            {
                Name = name,
                Email = email,
                Password = password
            };

            jsonData = serializer.SerializeUserData(user);

            if (jsonData == null)
            {
                Console.WriteLine("\nPlease try again.\n");
            }
        }

        Console.WriteLine("\nUser data validated and serialized successfully.");

        Console.WriteLine("\n--- Serialized Data ---");
        Console.WriteLine(jsonData);

        Console.WriteLine("\n--- Hash ---");
        Console.WriteLine(serializer.GetLastHash());

        Console.WriteLine("\n--- Deserialization from Trusted Source ---");
        var deserializedTrusted = serializer.DeserializeUserData(jsonData, trustedSource: true);
        if (deserializedTrusted != null)
        {
            Console.WriteLine($"Name: {deserializedTrusted.Name}, Email: {deserializedTrusted.Email}, Password: {deserializedTrusted.Password}");
        }

        Console.WriteLine("\n--- Deserialization from Untrusted Source ---");
        var deserializedUntrusted = serializer.DeserializeUserData(jsonData, trustedSource: false);
        Console.WriteLine(deserializedUntrusted == null ? "Blocked." : "Unexpected success!");
    }
}
