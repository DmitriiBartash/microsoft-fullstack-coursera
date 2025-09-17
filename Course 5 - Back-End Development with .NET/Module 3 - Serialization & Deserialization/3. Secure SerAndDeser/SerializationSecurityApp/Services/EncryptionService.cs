using System.Security.Cryptography;
using System.Text;

namespace SerializationSecurityApp.Services;

public static class EncryptionService
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("A1B2C3D4E5F6G7H8"); 
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("1H2G3F4E5D6C7B8A"); 

    public static string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(plainText);
        sw.Close();
        return Convert.ToBase64String(ms.ToArray());
    }
}
