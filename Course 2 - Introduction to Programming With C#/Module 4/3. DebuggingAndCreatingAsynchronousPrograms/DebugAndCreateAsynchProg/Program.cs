class Program
{
    // Step 2: Asynchronous method simulating a long operation
    static async Task PerformLongOperationAsync()
    {
        try
        {
            Console.WriteLine("Operation started...");
            await Task.Delay(3000); // Simulate delay (3 seconds)
            Console.WriteLine("Operation completed after delay.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    // Step 3: Main method - starting point of the app
    static void Main(string[] args)
    {
        // Run the async method and wait for it to complete
        Task task = Task.Run(() => PerformLongOperationAsync());
        task.Wait(); // Ensures the Main thread waits
        Console.WriteLine("Main method completed.");
    }
}
