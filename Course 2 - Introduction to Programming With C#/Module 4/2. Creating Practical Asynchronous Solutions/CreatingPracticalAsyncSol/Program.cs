using CreatingPracticalAsyncSol;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Running Program 1 ===");
        var p1 = new Problem1();
        await p1.DownloadFilesAsync();

        Console.WriteLine("\n=== Running Program 2 ===");
        var p2 = new Problem2();
        await p2.ProcessLargeDatasetAsync(5); // Example with 5 chunks

        Console.WriteLine("\n=== All Programs Finished ===");
    }
}
