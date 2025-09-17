using System.Security.Cryptography;
using System.Text;

namespace SerializationSecurityApp.Services;

public static class HashService
{
    public static string GenerateHash(string data)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(data);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
