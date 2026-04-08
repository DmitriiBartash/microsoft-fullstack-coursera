using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EncryptionApp.Services;

namespace EncryptionApp.Pages;

public class IndexModel : PageModel
{
    private readonly EncryptionService _encryptionService;
    private readonly IWebHostEnvironment _environment;

    public IndexModel(EncryptionService encryptionService, IWebHostEnvironment environment)
    {
        _encryptionService = encryptionService;
        _environment = environment;
    }

    [BindProperty]
    public IFormFile? UploadedFile { get; set; }

    public string? ResultMessage { get; set; }
    public string? ResultFilePath { get; set; }
    public string? OriginalFileName { get; set; }
    public bool IsSuccess { get; set; }

    public async Task<IActionResult> OnPostEncryptAsync()
    {
        if (UploadedFile is null || UploadedFile.Length == 0)
        {
            ResultMessage = "Please select a file to encrypt.";
            return Page();
        }

        if (Path.GetExtension(UploadedFile.FileName).Equals(".enc", StringComparison.OrdinalIgnoreCase))
        {
            ResultMessage = "This file is already encrypted (.enc). Use Decrypt instead.";
            return Page();
        }

        using var memoryStream = new MemoryStream();
        await UploadedFile.CopyToAsync(memoryStream);
        var plainData = memoryStream.ToArray();

        var encryptedData = _encryptionService.Encrypt(plainData);

        var encryptedDir = Path.Combine(_environment.WebRootPath, "encrypted");
        Directory.CreateDirectory(encryptedDir);

        var encryptedFileName = $"{Path.GetFileNameWithoutExtension(UploadedFile.FileName)}.enc";
        var encryptedFilePath = Path.Combine(encryptedDir, encryptedFileName);

        await System.IO.File.WriteAllBytesAsync(encryptedFilePath, encryptedData);

        IsSuccess = true;
        OriginalFileName = UploadedFile.FileName;
        ResultMessage = $"File \"{UploadedFile.FileName}\" encrypted successfully.";
        ResultFilePath = $"/encrypted/{encryptedFileName}";

        return Page();
    }

    public async Task<IActionResult> OnPostDecryptAsync()
    {
        if (UploadedFile is null || UploadedFile.Length == 0)
        {
            ResultMessage = "Please select an encrypted file (.enc) to decrypt.";
            return Page();
        }

        if (!Path.GetExtension(UploadedFile.FileName).Equals(".enc", StringComparison.OrdinalIgnoreCase))
        {
            ResultMessage = "Only encrypted files (.enc) can be decrypted. Please select a .enc file.";
            return Page();
        }

        using var memoryStream = new MemoryStream();
        await UploadedFile.CopyToAsync(memoryStream);
        var encryptedData = memoryStream.ToArray();

        byte[] decryptedData;
        try
        {
            decryptedData = _encryptionService.Decrypt(encryptedData);
        }
        catch (Exception)
        {
            ResultMessage = "Decryption failed. The file may be corrupted or was encrypted with a different key.";
            return Page();
        }

        var decryptedDir = Path.Combine(_environment.WebRootPath, "decrypted");
        Directory.CreateDirectory(decryptedDir);

        var originalName = Path.GetFileNameWithoutExtension(UploadedFile.FileName);
        var decryptedFileName = originalName.EndsWith(".enc", StringComparison.OrdinalIgnoreCase)
            ? originalName[..^4]
            : originalName;

        if (string.IsNullOrEmpty(Path.GetExtension(decryptedFileName)))
            decryptedFileName += ".txt";

        var decryptedFilePath = Path.Combine(decryptedDir, decryptedFileName);
        await System.IO.File.WriteAllBytesAsync(decryptedFilePath, decryptedData);

        IsSuccess = true;
        OriginalFileName = UploadedFile.FileName;
        ResultMessage = $"File decrypted successfully as \"{decryptedFileName}\".";
        ResultFilePath = $"/decrypted/{decryptedFileName}";

        return Page();
    }
}
