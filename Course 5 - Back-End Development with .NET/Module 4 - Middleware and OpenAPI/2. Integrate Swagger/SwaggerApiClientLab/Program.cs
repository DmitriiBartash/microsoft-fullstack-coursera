using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();

        _ = Task.Run(async () => await app.RunAsync());
        await Task.Delay(3000);

        if (!File.Exists("GeneratedApiClient.cs"))
        {
            var url = app.Urls.FirstOrDefault() ?? "http://localhost:5000";
            var generator = new ClientGenerator(url);
            await generator.GenerateClient();
            Console.WriteLine("GeneratedApiClient.cs created. Please restart the application.");
            return;
        }

        await RunClient(app);
    }

    private static async Task RunClient(WebApplication app)
    {
        var url = app.Urls.FirstOrDefault() ?? "http://localhost:5000";
        var assembly = Assembly.GetExecutingAssembly();
        var clientType = assembly.GetType("CustomNamespace.CustomApiClient");

        if (clientType == null)
        {
            Console.WriteLine("Client type not found. Please rebuild the project.");
            return;
        }

        using var http = new HttpClient();
        var client = Activator.CreateInstance(clientType, url, http);

        if (client == null)
        {
            Console.WriteLine("Failed to create client instance.");
            return;
        }

        var method = clientType.GetMethod("GetUserAsync", new[] { typeof(int), typeof(CancellationToken) })
                     ?? clientType.GetMethod("UserAsync", new[] { typeof(int) });

        if (method == null)
        {
            Console.WriteLine("API method not found in generated client.");
            return;
        }

        var parameters = method.GetParameters().Length == 2
            ? new object[] { 1, CancellationToken.None }
            : new object[] { 1 };

        var task = method.Invoke(client, parameters) as Task;

        if (task == null)
        {
            Console.WriteLine("Failed to invoke client method.");
            return;
        }

        await task.ConfigureAwait(false);

        var resultProperty = task.GetType().GetProperty("Result");
        if (resultProperty != null)
        {
            var user = resultProperty.GetValue(task);
            var idProp = user?.GetType().GetProperty("Id");
            var nameProp = user?.GetType().GetProperty("Name");

            Console.WriteLine($"User ID: {idProp?.GetValue(user)}, User Name: {nameProp?.GetValue(user)}");
        }
        else
        {
            Console.WriteLine("Request completed successfully (200 OK).");
        }
    }
}
