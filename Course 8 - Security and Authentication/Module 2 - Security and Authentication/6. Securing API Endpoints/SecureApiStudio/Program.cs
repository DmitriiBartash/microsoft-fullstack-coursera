using System.Security.Claims;
using SecureApiCore;
using SecureApiCore.Models;
using SecureApiCore.Services;
using SecureApiStudio.Components;
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

// The Studio calls its own in-process endpoints; trust the local dev HTTPS cert (demo only).
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

// ---- the real auth pipeline, hosted in-process, so the Studio exercises the genuine guard ----

app.MapPost("/auth/login", (LoginRequest req, int? seconds, UserStore users, TokenService tokens) =>
{
    var user = users.Find(req.Username, req.Password);
    if (user is null)
        return Results.Json(new MessageResponse("Invalid username or password."),
            statusCode: StatusCodes.Status401Unauthorized);

    var lifetime = seconds is > 0 ? TimeSpan.FromSeconds(seconds.Value) : (TimeSpan?)null;
    return Results.Ok(new TokenResponse(tokens.GenerateToken(user, lifetime)));
}).DisableAntiforgery();

// Public — no authorization requirement at all.
app.MapGet("/values/public", () =>
    Results.Ok(new MessageResponse("Public data — anyone can read this, no token required.")));

// Member — any caller in the Admin or User role.
app.MapGet("/values/user", (ClaimsPrincipal user) =>
    Results.Ok(new MessageResponse($"Member data for {user.Identity?.Name}. Requires the User or Admin role.")))
    .RequireAuthorization(policy => policy.RequireRole("Admin", "User"));

// Admin — Admin role only; a User-role token is rejected with 403.
app.MapGet("/values/admin", (ClaimsPrincipal user) =>
    Results.Ok(new MessageResponse($"Admin console for {user.Identity?.Name}. Requires the Admin role.")))
    .RequireAuthorization(policy => policy.RequireRole("Admin"));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
