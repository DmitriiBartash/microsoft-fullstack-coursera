using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(o => { });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = null;
        o.AccessDeniedPath = null;
        o.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
        o.Events.OnRedirectToAccessDenied = ctx =>
        {
            ctx.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminsOnly", p => p.RequireRole("Admin"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var method = context.Request.Method;
    var time = DateTime.Now.ToString("HH:mm:ss.fff");

    await next.Invoke();

    var status = context.Response.StatusCode;

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("──────────────────────────────");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[{time}] {method} {path}");
    Console.ForegroundColor = status >= 400 ? ConsoleColor.Red : ConsoleColor.Yellow;
    Console.WriteLine($"Status: {status}");
    Console.ResetColor();

    if (path == "/admin" && status == 200)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Conclusion: Admin access granted.");
        Console.ResetColor();
    }
    else if ((path == "/admin" || path == "/logout") && status == 403)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Conclusion: Access denied. User not authorized.");
        Console.ResetColor();
    }
});

app.Use(async (context, next) =>
{
    var watch = System.Diagnostics.Stopwatch.StartNew();
    await next.Invoke();
    watch.Stop();

    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine($"Duration: {watch.ElapsedMilliseconds} ms");
    Console.WriteLine("──────────────────────────────\n");
    Console.ResetColor();
});

app.MapGet("/", () => "Hello World");

// In practice this should be POST (but GET is used here for easy browser testing)
app.MapGet("/login", async (HttpContext ctx) =>
{
    var claims = new List<System.Security.Claims.Claim>
    {
        new(System.Security.Claims.ClaimTypes.Name, "TestUser"),
        new(System.Security.Claims.ClaimTypes.Role, "Admin")
    };
    var identity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new System.Security.Claims.ClaimsPrincipal(identity);
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    return Results.Ok("You are logged in as Admin.");
});

// In practice this should be POST (but GET is used here for easy browser testing)
app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok("You are logged out.");
}).RequireAuthorization("AdminsOnly");

app.MapGet("/admin", () => "Welcome, Admin!").RequireAuthorization("AdminsOnly");

app.MapGet("/error", () => Results.Problem("An error occurred"));

app.Run();
