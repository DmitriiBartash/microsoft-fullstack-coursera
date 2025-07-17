public class Problem1
{
    public async Task<string> DownloadFileAsync(string fileName)
    {
        Console.WriteLine($"Starting download of {fileName}...");
        await Task.Delay(3000); // Simulate a 3-second download time
        Console.WriteLine($"Completed download of {fileName}.");
        return $"{fileName} content";
    }

    public async Task DownloadFilesAsync()
    {
        // Start downloading "File1.txt" and "File2.txt" concurrently
        var downloadTask1 = DownloadFileAsync("File1.txt");
        var downloadTask2 = DownloadFileAsync("File2.txt");

        // Wait for both downloads to complete
        await Task.WhenAll(downloadTask1, downloadTask2);
        Console.WriteLine("All downloads completed.");
    }
}
