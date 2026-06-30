using System.Security.Cryptography;
using System.Text;

namespace SecureDataCore.Models;

// Holds one secret encrypted at rest (in memory) and gates decryption behind a role check.
public sealed class SecureStorage
{
    private readonly byte[] _key;
    private byte[]? _encrypted;

    public SecureStorage(byte[] key)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (key.Length is not (16 or 24 or 32))
            throw new ArgumentException("AES key must be 16, 24, or 32 bytes.", nameof(key));

        _key = key;
    }

    public bool HasData => _encrypted is not null;

    public void StoreData(string plaintext)
    {
        ArgumentNullException.ThrowIfNull(plaintext);
        _encrypted = Encrypt(Encoding.UTF8.GetBytes(plaintext));
    }

    // Raw bytes held at rest (IV + ciphertext) for proof/inspection; never decrypts.
    public byte[] GetEncryptedBytes()
        => _encrypted is null
            ? throw new InvalidOperationException("No data has been stored yet.")
            : (byte[])_encrypted.Clone();

    // Access control: only an Admin may decrypt and read the secret.
    public string RetrieveData(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        if (_encrypted is null)
            throw new InvalidOperationException("No data has been stored yet.");
        if (!string.Equals(user.Role, Roles.Admin, StringComparison.Ordinal))
            throw new UnauthorizedAccessException(
                $"Access denied for '{user.Username}' (role '{user.Role}'). Admin role required.");

        return Encoding.UTF8.GetString(Decrypt(_encrypted));
    }

    // Fresh random IV per call, prepended to the ciphertext.
    private byte[] Encrypt(byte[] plainData)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var cipher = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);

        var result = new byte[aes.IV.Length + cipher.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipher, 0, result, aes.IV.Length, cipher.Length);
        return result;
    }

    // Reads the IV back off the front, then decrypts the remainder.
    private byte[] Decrypt(byte[] ivAndCipher)
    {
        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = new byte[aes.BlockSize / 8];
        var cipher = new byte[ivAndCipher.Length - iv.Length];
        Buffer.BlockCopy(ivAndCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(ivAndCipher, iv.Length, cipher, 0, cipher.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
    }
}
