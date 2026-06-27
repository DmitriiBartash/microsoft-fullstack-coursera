using System.Net.Security;
using System.Security.Claims;
using JwtSecurityCore;
using JwtSecurityCore.Models;
using JwtSecurityCore.Services;
using JwtSecurityStudio.Components;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Missing 'JwtSettings' configuration section.");
jwtSettings.EnsureStrongKey(builder.Environment.IsDevelopment());
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<RefreshTokenStore>();
builder.Services.AddSingleton<UserStore>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = TokenService.BuildValidationParameters(jwtSettings);
    });
builder.Services.AddAuthorization();

// The Studio drives its own endpoints over HTTP with UseCookies=false, so the page captures and replays
// the refresh cookie explicitly — exactly what a browser would do. Certificate validation is bypassed
// only for the loopback self-call, never for a real remote host.
builder.Services.AddScoped(_ => new HttpClient(new HttpClientHandler
{
    UseCookies = false,
    ServerCertificateCustomValidationCallback = (request, _, _, errors) =>
        errors == SslPolicyErrors.None || (request.RequestUri?.IsLoopback ?? false)
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

// ---- the genuine auth pipeline, hosted in-process, so the Studio exercises real tokens & cookies ----

static void SetRefresh(HttpContext ctx, RefreshToken rt, bool httpOnly) =>
    ctx.Response.Cookies.Append(RefreshCookieDefaults.Name, rt.Token, new CookieOptions
    {
        HttpOnly = httpOnly,
        Secure = ctx.Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Path = RefreshCookieDefaults.Path,
        Expires = rt.ExpiresUtc
    });

static int ExpiresIn(int? seconds, JwtSettings settings) =>
    seconds is > 0 ? seconds.Value : settings.AccessTokenMinutes * 60;

static TimeSpan? Lifetime(int? seconds) =>
    seconds is > 0 ? TimeSpan.FromSeconds(seconds.Value) : null;

app.MapPost("/auth/login", (LoginRequest req, int? seconds, bool? httpOnly,
    HttpContext ctx, UserStore users, TokenService tokens, RefreshTokenStore refresh, JwtSettings settings) =>
{
    var user = users.Find(req.Username, req.Password);
    if (user is null)
        return Results.Json(new MessageResponse("Invalid username or password."), statusCode: StatusCodes.Status401Unauthorized);

    var hardened = httpOnly ?? true;
    var rt = refresh.Issue(user.Username);
    SetRefresh(ctx, rt, hardened);

    var leaked = hardened ? null : rt.Token;
    return Results.Ok(new TokenResponse(tokens.GenerateToken(user, Lifetime(seconds)), ExpiresIn(seconds, settings), leaked));
}).DisableAntiforgery();

app.MapPost("/auth/refresh", (bool? rotate, int? seconds, bool? httpOnly,
    HttpContext ctx, UserStore users, TokenService tokens, RefreshTokenStore refresh, JwtSettings settings) =>
{
    var presented = ctx.Request.Cookies[RefreshCookieDefaults.Name];
    var next = (rotate ?? true) ? refresh.Rotate(presented) : refresh.Validate(presented);
    var user = next is null ? null : users.All.FirstOrDefault(u => u.Username == next.Username);

    if (next is null || user is null)
    {
        ctx.Response.Cookies.Delete(RefreshCookieDefaults.Name, new CookieOptions { Path = RefreshCookieDefaults.Path });
        return Results.Json(new MessageResponse("Refresh token missing, expired, or already used."),
            statusCode: StatusCodes.Status401Unauthorized);
    }

    SetRefresh(ctx, next, httpOnly ?? true);
    return Results.Ok(new TokenResponse(tokens.GenerateToken(user, Lifetime(seconds)), ExpiresIn(seconds, settings)));
}).DisableAntiforgery();

app.MapPost("/auth/logout", (HttpContext ctx, RefreshTokenStore refresh) =>
{
    refresh.Revoke(ctx.Request.Cookies[RefreshCookieDefaults.Name]);
    ctx.Response.Cookies.Delete(RefreshCookieDefaults.Name, new CookieOptions { Path = RefreshCookieDefaults.Path });
    return Results.Ok(new MessageResponse("Logged out — refresh token revoked."));
}).DisableAntiforgery();

app.MapGet("/secure", (ClaimsPrincipal user) =>
    Results.Ok(new MessageResponse($"Protected data for {user.Identity?.Name}. A valid access token unlocked this.")))
    .RequireAuthorization();

app.MapGet("/secure/admin", (ClaimsPrincipal user) =>
    Results.Ok(new MessageResponse($"Admin area for {user.Identity?.Name}. Requires the Admin role.")))
    .RequireAuthorization(policy => policy.RequireRole("Admin"));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
