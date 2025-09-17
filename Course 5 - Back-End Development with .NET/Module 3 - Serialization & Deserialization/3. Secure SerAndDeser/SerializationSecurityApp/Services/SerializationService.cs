using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using SerializationSecurityApp.Models;

namespace SerializationSecurityApp.Services;

public class SerializationService
{
    private string? _lastHash;

    public string? SerializeUserData(User user)
    {
        var validationContext = new ValidationContext(user);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(user, validationContext, results, true);

        if (!isValid)
        {
            Console.WriteLine("\nValidation errors:");
            foreach (var error in results)
            {
                Console.WriteLine($" - {error.ErrorMessage}");
            }
            return null;
        }

        user.Password = EncryptionService.Encrypt(user.Password);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var jsonData = JsonSerializer.Serialize(user, options);
        _lastHash = HashService.GenerateHash(jsonData);
        return jsonData;
    }

    public User? DeserializeUserData(string jsonData, bool trustedSource)
    {
        if (!trustedSource)
        {
            Console.WriteLine("Untrusted source. Deserialization aborted.");
            return null;
        }

        var hash = HashService.GenerateHash(jsonData);
        if (_lastHash != hash)
        {
            Console.WriteLine("Data integrity check failed. Possible tampering detected.");
            return null;
        }

        return JsonSerializer.Deserialize<User>(jsonData);
    }

    public string? GetLastHash() => _lastHash;
}
