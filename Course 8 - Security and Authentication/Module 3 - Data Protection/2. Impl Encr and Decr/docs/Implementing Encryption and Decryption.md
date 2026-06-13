# Implementing Encryption and Decryption

**Course 8 — Security and Authentication** · Module 3 · Lesson 2 · `You Try It!`

> Build a small **ASP.NET Core Razor Pages** app that lets a user upload a file,
> **encrypt** it with **AES**, and later **decrypt** it back to the original.
> The encryption logic lives in a dedicated `EncryptionService`, and the Razor page
> just collects the file and calls the service.

---

## 🎯 Objective

Create an ASP.NET Core web application that allows users to encrypt and decrypt files.
This exercise demonstrates how to implement basic encryption and decryption techniques
using the `System.Security.Cryptography` **AES** primitives together with file I/O.

---

## 🗂️ What you'll build

A Razor Pages project named **`EncryptionApp`** with these pieces:

| File                  | Responsibility                                                     |
| --------------------- | ----------------------------------------------------------------- |
| `EncryptionService.cs`| Encrypt and decrypt files with a symmetric **AES** key            |
| `Pages/Index.cshtml`  | Upload form with **Encrypt** and **Decrypt** buttons              |
| `Pages/Index.cshtml.cs`| Handle the upload, call the service, write results to `wwwroot`  |

**Flow:** `Upload file  →  EncryptionService.EncryptFile()  →  .enc in wwwroot  →  EncryptionService.DecryptFile()  →  recovered file`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code with the C# extension
- A terminal open at the folder where you want the project

---

## 🛠️ Steps

### Step 1 — Prepare the application

Scaffold a Razor Pages app, move into it, restore, build, and run it once to confirm the setup.

```bash
dotnet new razor -n EncryptionApp
cd EncryptionApp
dotnet restore
dotnet build
dotnet run
```

Open `http://localhost:5xxx` in a browser to confirm the default page loads, then stop the app
(`Ctrl+C`). You'll edit `Pages/Index.cshtml` shortly to add a file-upload form.

### Step 2 — Create an encryption service

Create a new file `EncryptionService.cs` in the project root. It defines two methods — one to
**encrypt** a file with a symmetric AES key and one to **decrypt** a previously encrypted file.
The AES IV (initialization vector) is generated per encryption and written to the front of the
output file so decryption can read it back.

```csharp
using System.Security.Cryptography;

namespace EncryptionApp;

public class EncryptionService
{
    // Demo key only. In production, load this from configuration / a key vault,
    // never hard-code it. AES-256 needs a 32-byte key.
    private static readonly byte[] Key = SHA256.HashData(
        System.Text.Encoding.UTF8.GetBytes("super-secret-demo-passphrase"));

    // Encrypt a file using a symmetric key; prefix the output with the IV.
    public async Task EncryptFileAsync(string inputPath, string outputPath)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.GenerateIV();

        await using var output = new FileStream(outputPath, FileMode.Create);
        // Store the IV (16 bytes) at the start so we can decrypt later.
        await output.WriteAsync(aes.IV);

        await using var crypto = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await using var input = new FileStream(inputPath, FileMode.Open);
        await input.CopyToAsync(crypto);
    }

    // Decrypt a previously encrypted file by reading the IV back off the front.
    public async Task DecryptFileAsync(string inputPath, string outputPath)
    {
        using var aes = Aes.Create();
        aes.Key = Key;

        await using var input = new FileStream(inputPath, FileMode.Open);
        var iv = new byte[aes.IV.Length];
        await input.ReadExactlyAsync(iv);
        aes.IV = iv;

        await using var crypto = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
        await using var output = new FileStream(outputPath, FileMode.Create);
        await crypto.CopyToAsync(output);
    }
}
```

Register the service for dependency injection in `Program.cs`:

```csharp
builder.Services.AddSingleton<EncryptionService>();
```

### Step 3 — Add UI for file upload

Replace the body of `Pages/Index.cshtml` with a form that has a file input and two submit
buttons — one for **Encrypt** and one for **Decrypt**.

```html
@page
@model IndexModel

<h2>File Encryption / Decryption</h2>

<form method="post" enctype="multipart/form-data">
    <input type="file" name="upload" />
    <button type="submit" asp-page-handler="Encrypt">Encrypt</button>
    <button type="submit" asp-page-handler="Decrypt">Decrypt</button>
</form>

@if (!string.IsNullOrEmpty(Model.Message))
{
    <p>@Model.Message</p>
}
@if (!string.IsNullOrEmpty(Model.DownloadFile))
{
    <a href="@Model.DownloadFile" download>Download result</a>
}
```

### Step 4 — Implement file encryption

Update `Pages/Index.cshtml.cs` to handle the upload and call the service. The `OnPostEncryptAsync`
handler saves the upload to a temp file, runs AES encryption, and writes the encrypted file to the
`wwwroot` directory so it can be served back to the user.

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EncryptionApp.Pages;

public class IndexModel : PageModel
{
    private readonly EncryptionService _crypto;
    private readonly IWebHostEnvironment _env;

    public IndexModel(EncryptionService crypto, IWebHostEnvironment env)
    {
        _crypto = crypto;
        _env = env;
    }

    public string? Message { get; set; }
    public string? DownloadFile { get; set; }

    public async Task<IActionResult> OnPostEncryptAsync(IFormFile? upload)
    {
        if (upload is null || upload.Length == 0)
        {
            Message = "Please choose a file first.";
            return Page();
        }

        var tempPath = Path.GetTempFileName();
        await using (var stream = System.IO.File.Create(tempPath))
        {
            await upload.CopyToAsync(stream);
        }

        var outputName = upload.FileName + ".enc";
        var outputPath = Path.Combine(_env.WebRootPath, outputName);
        await _crypto.EncryptFileAsync(tempPath, outputPath);
        System.IO.File.Delete(tempPath);

        Message = $"Encrypted to wwwroot/{outputName}";
        DownloadFile = "/" + outputName;
        return Page();
    }
}
```

### Step 5 — Implement file decryption

Add a matching `OnPostDecryptAsync` handler to the same `IndexModel`. It reads the uploaded
`.enc` file, calls `DecryptFileAsync`, and saves the recovered file to `wwwroot` for download.

```csharp
    public async Task<IActionResult> OnPostDecryptAsync(IFormFile? upload)
    {
        if (upload is null || upload.Length == 0)
        {
            Message = "Please choose an encrypted (.enc) file first.";
            return Page();
        }

        var tempPath = Path.GetTempFileName();
        await using (var stream = System.IO.File.Create(tempPath))
        {
            await upload.CopyToAsync(stream);
        }

        var outputName = "decrypted-" + Path.GetFileNameWithoutExtension(upload.FileName);
        var outputPath = Path.Combine(_env.WebRootPath, outputName);
        await _crypto.DecryptFileAsync(tempPath, outputPath);
        System.IO.File.Delete(tempPath);

        Message = $"Decrypted to wwwroot/{outputName}";
        DownloadFile = "/" + outputName;
        return Page();
    }
```

### Step 6 — Test the application

Run the app and verify the full round trip.

```bash
dotnet run
```

- Create a test file: open Notepad (or any editor), type some text such as `This is a test file`,
  and save it as `testfile.txt`.
- In the app, choose `testfile.txt` and click **Encrypt** — confirm `testfile.txt.enc` appears in
  the `wwwroot` directory.
- Choose the `.enc` file and click **Decrypt** — open the recovered file from `wwwroot` and confirm
  its contents match the original.

---

## ▶️ Expected result

Uploading `testfile.txt` and clicking **Encrypt** writes an unreadable `testfile.txt.enc` into
`wwwroot`. Feeding that `.enc` file back through **Decrypt** reproduces the original text exactly —
proving the AES encrypt/decrypt round trip works end to end.

---

## ☑️ Definition of done

- [ ] `EncryptionApp` Razor Pages project created, builds, and runs
- [ ] `EncryptionService.cs` encrypts a file with a symmetric **AES** key
- [ ] `EncryptionService.cs` decrypts a previously encrypted file back to the original
- [ ] `Index.cshtml` shows a file input with **Encrypt** and **Decrypt** buttons
- [ ] `Index.cshtml.cs` handles uploads and writes results to the `wwwroot` directory
- [ ] A `.txt` file survives an encrypt → decrypt round trip with identical contents

---

## 🔑 Key concepts

- **Symmetric encryption (AES)** — the *same* key encrypts and decrypts. `Aes.Create()` from
  `System.Security.Cryptography` gives you a vetted, hardware-accelerated implementation; never roll
  your own cipher.
- **The IV is not secret, but must be unique** — generate a fresh initialization vector per
  encryption and store it alongside the ciphertext (here, at the front of the file) so decryption can
  reconstruct it. Reusing an IV with the same key leaks information.
- **`CryptoStream` streams the work** — wrapping a `FileStream` in a `CryptoStream` encrypts/decrypts
  data as it flows, so large files never have to be fully loaded into memory.
- **Keep keys out of source** — the demo derives a key from a passphrase for clarity, but real apps
  load keys from configuration, environment variables, or a key vault, and rotate them.
- **Separation of concerns** — the cryptography lives in `EncryptionService`, while the Razor page
  only handles HTTP and file I/O, so the crypto logic is reusable and testable on its own.
