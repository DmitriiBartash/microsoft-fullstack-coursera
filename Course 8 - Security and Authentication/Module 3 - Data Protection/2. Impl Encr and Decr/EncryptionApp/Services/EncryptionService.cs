using System.Security.Cryptography;

namespace EncryptionApp.Services;

public class EncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(IConfiguration configuration)
    {
        var keyBase64 = configuration["Encryption:Key"]
            ?? throw new InvalidOperationException("Encryption key is not configured.");

        _key = Convert.FromBase64String(keyBase64);
    }

    // Encrypts data and prepends the IV to the output
    public byte[] Encrypt(byte[] plainData)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var encryptedData = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);

        // IV (16 bytes) + encrypted data
        var result = new byte[aes.IV.Length + encryptedData.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encryptedData, 0, result, aes.IV.Length, encryptedData.Length);

        return result;
    }

    // Reads IV from the first 16 bytes, then decrypts the rest
    public byte[] Decrypt(byte[] encryptedDataWithIv)
    {
        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = new byte[aes.BlockSize / 8];
        var cipherText = new byte[encryptedDataWithIv.Length - iv.Length];

        Buffer.BlockCopy(encryptedDataWithIv, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(encryptedDataWithIv, iv.Length, cipherText, 0, cipherText.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
    }
}
