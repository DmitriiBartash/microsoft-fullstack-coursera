using System.Security.Claims;
using JwtAuthCore;
using JwtAuthCore.Models;
using JwtAuthCore.Services;
using JwtAuthStudio.Components;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Missing 'JwtSettings' configuration section.");
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<UserStore>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = TokenService.BuildValidationParameters(jwtSettings);
    });
builder.Services.AddAuthorization();

// Trust the local dev HTTPS cert for the Studio's calls to its own endpoints (demo only).
builder.Services.AddScoped(_ => new HttpClient(new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
}));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Login + protected endpoints hosted in-process, so the Studio exercises the real JwtBearer guard.
app.MapPost("/login", (LoginRequest req, int? seconds, UserStore users, TokenService tokens) =>
{
    var user = users.Find(req.Username, req.Password);
    if (user is null)
        return Results.Json(new MessageResponse("Invalid username or password."),
            statusCode: StatusCodes.Status401Unauthorized);

    var lifetime = seconds is > 0 ? TimeSpan.FromSeconds(seconds.Value) : (TimeSpan?)null;
    return Results.Ok(new TokenResponse(tokens.GenerateToken(user, lifetime)));
}).DisableAntiforgery();

app.MapGet("/secure-data", (ClaimsPrincipal user) =>
    Results.Ok(new MessageResponse($"You reached the secure endpoint, {user.Identity?.Name}. Your token checked out.")))
    .RequireAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public partial class Program;
