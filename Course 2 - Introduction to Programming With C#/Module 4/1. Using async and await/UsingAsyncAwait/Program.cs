namespace UsingAsyncAwait
{
    class Program
    {
        // Step 2: First asynchronous method
        public async Task DownloadDataAsync()
        {
            try
            {
                Console.WriteLine("DownloadDataAsync started ...");
                await Task.Delay(3000); // Simulate 3-second delay
                throw new Exception("Simulated download error.");
                Console.WriteLine("DownloadDataAsync completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DownloadDataAsync: {ex.Message}");
            }
        }

        // Step 4: Second asynchronous method
        public async Task DownloadDataAsync2()
        {
            Console.WriteLine("DownloadDataAsync2 started...");
            await Task.Delay(2000); // Simulate 2-second delay
            Console.WriteLine("DownloadDataAsync2 completed");
        }

        // Step 3 + 4: Main method (entry point)
        static async Task Main(string[] args)
        {
            Program program = new();

            // Step 4: Run both async methods in parallel
            Task task1 = program.DownloadDataAsync();
            Task task2 = program.DownloadDataAsync2();

            await Task.WhenAll(task1, task2);
            Console.WriteLine("All downloads completed.");

        }
    }
}