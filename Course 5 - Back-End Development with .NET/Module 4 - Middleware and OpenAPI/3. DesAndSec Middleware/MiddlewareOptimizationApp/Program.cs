using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5294);
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (!context.Request.Query.TryGetValue("secure", out var secure) || secure != "true")
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Simulated HTTPS Required");
        Console.WriteLine($"Security Event: {context.Request.Path} - Status Code: 400");
        return;
    }
    await next();
});

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/unauthorized"))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized Access");
        Console.WriteLine($"Security Event: {context.Request.Path} - Status Code: 401");
        return;
    }
    await next();
});

app.Use(async (context, next) =>
{
    if (context.Request.Query.TryGetValue("input", out var input))
    {
        if (Regex.IsMatch(input.ToString(), "<.*?>"))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid Input");
            Console.WriteLine($"Security Event: {context.Request.Path} - Status Code: 400");
            return;
        }
    }
    await next();
});

app.Use(async (context, next) =>
{
    if (!context.Request.Query.TryGetValue("authenticated", out var authenticated) || authenticated != "true")
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Access Denied");
        Console.WriteLine($"Security Event: {context.Request.Path} - Status Code: 403");
        return;
    }
    await next();
});

app.Use(async (context, next) =>
{
    await Task.Delay(100);
    await context.Response.WriteAsync("Processed Asynchronously\n");
    await next();
});

app.Map("/", async context =>
{
    await context.Response.WriteAsync("Final Response from Application");
});

app.Run();
