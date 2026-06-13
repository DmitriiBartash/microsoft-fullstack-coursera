# Securing Serialization and Deserialization

**Course 5 — Back-End Development with .NET** · Module 3 · Lesson 3 · `You Try It!`

> Build a .NET console app that serializes user data **securely**. You'll walk the
> full threat surface — exposed secrets, deserialization attacks, tampering — and
> layer in **input validation**, **AES encryption**, **trusted-source checks**, and a
> **SHA-256 integrity hash**, organized into clean `Models` and `Services` classes.

---

## 🎯 Objective

By the end of this lab you will be able to identify potential security risks in
serialization, implement secure serialization practices, validate input data with
data annotations, use a secure library (`System.Text.Json`), encrypt sensitive
fields before serialization, and verify the integrity of serialized data in .NET.

---

## 🗂️ What you will build

A console project named **`SerializationSecurityApp`** organized into a model and three services:

| File                                | Responsibility                                                              |
| ----------------------------------- | --------------------------------------------------------------------------- |
| `Models/User.cs`                    | The `User` model with `[Required]` / `[EmailAddress]` / `[StringLength]` validation |
| `Services/EncryptionService.cs`     | AES-encrypt sensitive fields (the `Password`) before serialization          |
| `Services/HashService.cs`           | Produce a SHA-256 hash of serialized data for integrity checks              |
| `Services/SerializationService.cs`  | Validate → encrypt → serialize, and verify-source → check-hash → deserialize |
| `Program.cs`                        | Drive the flow: collect input, serialize, then deserialize trusted vs. untrusted |

**Flow:** `Input → Validate → Encrypt(Password) → Serialize(JSON) → Hash → Deserialize(if trusted & hash matches)`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- Built-in namespaces only: `System.Text.Json`, `System.Security.Cryptography`, `System.ComponentModel.DataAnnotations` (no extra NuGet packages)

---

## 🛠️ Steps

### Step 1 — Create a new console application

Scaffold the project and move into it.

```bash
dotnet new console -o SerializationSecurityApp
cd SerializationSecurityApp
```

Open `Program.cs` in Visual Studio Code and delete any existing code — each step adds to a clean project.

### Step 2 — Identify serialization risks

Before securing anything, understand what goes wrong. Serializing a raw `User` to JSON would:

- **Expose sensitive data** — the `Password` lands in the output as plain text.
- **Enable deserialization attacks** — accepting JSON from any source lets an attacker craft malicious payloads.
- **Allow data tampering** — without an integrity check, a modified payload deserializes as if it were genuine.

The remaining steps neutralize each of these risks.

### Step 3 — Implement input validation for serialization

Add validation attributes to the model so malformed data never reaches serialization. Create **`Models/User.cs`**:

```csharp
using System.ComponentModel.DataAnnotations;

namespace SerializationSecurityApp.Models;

public class User
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public required string Password { get; set; }
}
```

Validation itself runs inside the serialization service (Step 6) via `Validator.TryValidateObject`; if any property fails, the object is **not** serialized and the errors are reported.

### Step 4 — Use a secure library for serialization

Use the built-in **`System.Text.Json`** library rather than legacy/binary formatters (which are unsafe for untrusted input). It is fast, does not execute arbitrary types, and is configured explicitly. The lab enables:

- `WriteIndented = true` for readable output.
- `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull` so null fields are omitted.

This is wired up in `SerializationService.SerializeUserData` (Step 6).

### Step 5 — Encrypt sensitive data before serialization

Encrypt the `Password` before it is ever written to JSON. Create **`Services/EncryptionService.cs`**:

```csharp
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
```

> The 16-byte `Key`/`IV` are hard-coded here **only for the lab**. In production, load them from a secret store (environment variables, Azure Key Vault, etc.) and use a fresh random IV per message — never commit keys to source control.

### Step 6 — Prevent deserialization of untrusted data

Build the core service that ties validation, encryption, serialization, hashing, and trusted-source checks together. First add **`Services/HashService.cs`** for the integrity hash (used here and in Step 7):

```csharp
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
```

Now create **`Services/SerializationService.cs`**. `DeserializeUserData` refuses any **untrusted source** and aborts deserialization entirely:

```csharp
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
```

### Step 7 — Implement data integrity checks

Notice the integrity check is already woven in above: `SerializeUserData` stores `_lastHash = HashService.GenerateHash(jsonData)` right after serializing, and `DeserializeUserData` recomputes the hash of the incoming JSON and **rejects** it if it differs from `_lastHash` — detecting any tampering in transit before the object is reconstructed.

Finally, wire everything together in **`Program.cs`**:

```csharp
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
```

Run it:

```bash
dotnet run
```

---

## ▶️ Expected result

Enter valid details (Name ≥ 3 chars, a real email, Password ≥ 6 chars) and the app prints:

- **Serialized Data** — indented JSON where `Password` is an AES Base64 ciphertext, not plain text.
- **Hash** — the SHA-256 (Base64) integrity hash of that JSON.
- **Deserialization from Trusted Source** — succeeds and prints the reconstructed `Name`, `Email`, and (encrypted) `Password`.
- **Deserialization from Untrusted Source** — prints `Untrusted source. Deserialization aborted.` then `Blocked.`

Entering invalid data first prints the validation errors and re-prompts until the input is valid.

---

## ☑️ Definition of done

- [ ] `SerializationSecurityApp` console project created with `Models/` and `Services/` folders
- [ ] `User` model carries `[Required]`, `[EmailAddress]`, and `[StringLength]` validation attributes
- [ ] `SerializeUserData` rejects invalid objects via `Validator.TryValidateObject` and re-prompts
- [ ] `Password` is AES-encrypted by `EncryptionService` before it is serialized
- [ ] Serialization uses `System.Text.Json` with `WriteIndented` and `WhenWritingNull`
- [ ] A SHA-256 hash is stored on serialize and re-checked on deserialize
- [ ] Deserialization is blocked for untrusted sources and on any hash mismatch
- [ ] `dotnet run` shows trusted deserialization succeeding and untrusted being blocked

---

## 🔑 Key concepts

- **Validate before you serialize** — `Validator.TryValidateObject` against data annotations means malformed or oversized input never enters the pipeline (fail fast), shrinking the attack surface.
- **Never trust incoming payloads** — gate deserialization on a trusted-source flag and use a safe serializer (`System.Text.Json`) instead of binary formatters that can instantiate arbitrary types.
- **Protect data in transit and at rest** — encrypt sensitive fields (the `Password`) *before* serialization so secrets are never written in plain text, even if the JSON leaks.
- **Integrity via hashing** — storing a SHA-256 hash at serialize time and re-comparing it at deserialize time detects tampering before the object is reconstructed.
- **Keep secrets out of code** — the lab's hard-coded AES `Key`/`IV` are for learning only; production keys belong in a secret store with a per-message random IV.
