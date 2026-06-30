using Microsoft.Extensions.Configuration;
using SecureDataCore.Models;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var keyBase64 = config["Encryption:Key"]
    ?? throw new InvalidOperationException("Encryption:Key is not configured in appsettings.json.");

var storage = new SecureStorage(Convert.FromBase64String(keyBase64));

var admin = new User("alice", Roles.Admin);
var regular = new User("bob", Roles.User);

const string secret = "Customer SSN: 123-45-6789";

Console.WriteLine("=== Secure Data Storage demo ===\n");

// Store sensitive data — encrypted before it ever sits in memory.
storage.StoreData(secret);
Console.WriteLine($"Stored plaintext : {secret}");
Console.WriteLine($"At rest (base64) : {Convert.ToBase64String(storage.GetEncryptedBytes())}");
Console.WriteLine("  -> the bytes above are ciphertext (IV + AES); plaintext is not recoverable from them alone.\n");

// Admin is allowed to decrypt and read; non-Admin is denied and the data stays encrypted.
TryRetrieve(admin);
TryRetrieve(regular);

void TryRetrieve(User user)
{
    Console.Write($"{user.Username} (role '{user.Role}') requests the data -> ");
    try
    {
        var data = storage.RetrieveData(user);
        Console.WriteLine($"GRANTED: \"{data}\"");
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine($"DENIED: {ex.Message}");
    }
}
