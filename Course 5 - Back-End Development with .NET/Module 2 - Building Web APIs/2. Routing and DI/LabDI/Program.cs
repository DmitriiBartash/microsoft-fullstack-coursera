var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IMyServiceSingleton, MyServiceSingleton>();
builder.Services.AddScoped<IMyServiceScoped, MyServiceScoped>();
builder.Services.AddTransient<IMyServiceTransient, MyServiceTransient>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.RequestServices.GetRequiredService<IMyServiceSingleton>().LogCreation("Middleware 1");
    context.RequestServices.GetRequiredService<IMyServiceScoped>().LogCreation("Middleware 1");
    context.RequestServices.GetRequiredService<IMyServiceTransient>().LogCreation("Middleware 1");
    await next.Invoke();
});

app.Use(async (context, next) =>
{
    context.RequestServices.GetRequiredService<IMyServiceSingleton>().LogCreation("Middleware 2");
    context.RequestServices.GetRequiredService<IMyServiceScoped>().LogCreation("Middleware 2");
    context.RequestServices.GetRequiredService<IMyServiceTransient>().LogCreation("Middleware 2");
    await next.Invoke();
});

app.MapGet("/", (
    IMyServiceSingleton singleton,
    IMyServiceScoped scoped,
    IMyServiceTransient transient) =>
{
    var output = new List<string>();

    output.Add(singleton.GetInfo("Root endpoint"));
    output.Add(scoped.GetInfo("Root endpoint"));
    output.Add(transient.GetInfo("Root endpoint"));

    return string.Join("\n", output);
});

app.Run();

public interface IMyServiceSingleton
{
    void LogCreation(string source);
    string GetInfo(string source);
}
public interface IMyServiceScoped
{
    void LogCreation(string source);
    string GetInfo(string source);
}
public interface IMyServiceTransient
{
    void LogCreation(string source);
    string GetInfo(string source);
}

public class MyServiceSingleton : IMyServiceSingleton
{
    private readonly int _serviceId = new Random().Next(100000, 999999);
    public void LogCreation(string source)
    {
        Console.WriteLine(GetInfo(source));
    }
    public string GetInfo(string source)
    {
        return $"[Singleton | ID: {_serviceId}] → {source} | Explanation: Same instance across the whole application lifetime";
    }
}

public class MyServiceScoped : IMyServiceScoped
{
    private readonly int _serviceId = new Random().Next(100000, 999999);
    public void LogCreation(string source)
    {
        Console.WriteLine(GetInfo(source));
    }
    public string GetInfo(string source)
    {
        return $"[Scoped | ID: {_serviceId}] → {source} | Explanation: Same instance reused within one HTTP request";
    }
}

public class MyServiceTransient : IMyServiceTransient
{
    private readonly int _serviceId = new Random().Next(100000, 999999);
    public void LogCreation(string source)
    {
        Console.WriteLine(GetInfo(source));
    }
    public string GetInfo(string source)
    {
        return $"[Transient | ID: {_serviceId}] → {source} | Explanation: Always a new instance, even within the same request";
    }
}
