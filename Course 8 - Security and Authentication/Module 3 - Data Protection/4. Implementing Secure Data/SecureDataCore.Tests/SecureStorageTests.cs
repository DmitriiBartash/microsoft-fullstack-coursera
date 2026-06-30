using System.Security.Cryptography;
using System.Text;
using SecureDataCore.Models;

namespace SecureDataCore.Tests;

public class SecureStorageTests
{
    // Fixed AES-256 key so every test run is deterministic.
    private static byte[] TestKey() => SHA256.HashData("unit-test-key"u8.ToArray());

    private static User Admin() => new("root", Roles.Admin);
    private static User Regular() => new("guest", Roles.User);

    [Fact]
    public void Admin_can_store_then_retrieve_the_original_plaintext()
    {
        var storage = new SecureStorage(TestKey());
        const string secret = "Top secret: launch codes 0000";

        storage.StoreData(secret);
        var result = storage.RetrieveData(Admin());

        Assert.Equal(secret, result);
    }

    [Fact]
    public void Stored_bytes_are_ciphertext_not_the_plaintext()
    {
        var storage = new SecureStorage(TestKey());
        const string secret = "Sensitive PII: 123-45-6789";

        storage.StoreData(secret);
        var atRest = storage.GetEncryptedBytes();

        Assert.DoesNotContain(secret, Encoding.UTF8.GetString(atRest));
        Assert.True(atRest.Length > secret.Length, "ciphertext should carry IV + padding overhead");
    }

    [Fact]
    public void Non_admin_retrieval_is_denied()
    {
        var storage = new SecureStorage(TestKey());
        storage.StoreData("Top secret");

        Assert.Throws<UnauthorizedAccessException>(() => storage.RetrieveData(Regular()));
    }

    [Fact]
    public void Same_plaintext_produces_different_ciphertext_due_to_random_iv()
    {
        var a = new SecureStorage(TestKey());
        var b = new SecureStorage(TestKey());

        a.StoreData("same input");
        b.StoreData("same input");

        Assert.NotEqual(a.GetEncryptedBytes(), b.GetEncryptedBytes());
    }

    [Fact]
    public void Constructor_rejects_an_invalid_key_size()
    {
        Assert.Throws<ArgumentException>(() => new SecureStorage(new byte[10]));
    }

    [Fact]
    public void Retrieving_before_storing_throws()
    {
        var storage = new SecureStorage(TestKey());

        Assert.Throws<InvalidOperationException>(() => storage.RetrieveData(Admin()));
    }
}
