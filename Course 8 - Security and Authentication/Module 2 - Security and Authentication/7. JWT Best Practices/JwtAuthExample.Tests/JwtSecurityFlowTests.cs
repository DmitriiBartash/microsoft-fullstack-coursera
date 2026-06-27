using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using JwtSecurityCore;
using JwtSecurityCore.Models;
using JwtSecurityCore.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthExample.Tests;

// Drives the real pipeline (JwtBearer + [Authorize] + refresh-token rotation) over an in-memory TestServer.
// Each test builds its OWN factory, so the singleton RefreshTokenStore/UserStore are isolated per test and
// reuse-detection's RevokeAllFor in one test cannot affect another. HandleCookies is disabled so each test
// controls exactly which refresh cookie it replays.
public class JwtSecurityFlowTests
{
    private static WebApplicationFactory<Program> NewApp() => new();

    private static HttpClient Client(WebApplicationFactory<Program> factory) =>
        factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });

    // ---------- login ----------

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAccessToken()
    {
        using var factory = NewApp();
        var client = Client(factory);

        var resp = await client.PostAsJsonAsync("/auth/login", new LoginRequest("admin", "password"));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var body = await resp.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.False(string.IsNullOrWhiteSpace(body?.Token));
        Assert.True(body!.ExpiresInSeconds > 0);
        Assert.Null(body.RefreshToken); // hardened flow: refresh travels in the cookie, never the body
    }

    [Fact]
    public async Task Login_WithBadCredentials_Returns401()
    {
        using var factory = NewApp();
        var client = Client(factory);

        var resp = await client.PostAsJsonAsync("/auth/login", new LoginRequest("admin", "WRONG"));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Login_SetsHttpOnlyRefreshCookie_ScopedToAuth()
    {
        using var factory = NewApp();
        var client = Client(factory);

        var resp = await client.PostAsJsonAsync("/auth/login", new LoginRequest("admin", "password"));

        var setCookie = resp.Headers.GetValues("Set-Cookie").Single(c => c.StartsWith("refresh_token="));
        Assert.Contains("httponly", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("path=/auth", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=strict", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.False(string.IsNullOrEmpty(RefreshValue(resp)));
    }

    // ---------- protected route ----------

    [Fact]
    public async Task Secure_WithoutToken_Returns401()
    {
        using var factory = NewApp();
        var client = Client(factory);

        var resp = await client.SendAsync(Get("/secure"));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Secure_WithAccessToken_Returns200()
    {
        using var factory = NewApp();
        var client = Client(factory);
        var token = await AccessTokenFor(client, "alice", "pa55");

        var resp = await client.SendAsync(Get("/secure", token));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task SecureAdmin_WithAdminToken_Returns200()
    {
        using var factory = NewApp();
        var client = Client(factory);
        var token = await AccessTokenFor(client, "admin", "password");

        var resp = await client.SendAsync(Get("/secure/admin", token));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task SecureAdmin_WithUserToken_Returns403()
    {
        using var factory = NewApp();
        var client = Client(factory);
        var token = await AccessTokenFor(client, "alice", "pa55"); // alice is User, not Admin

        var resp = await client.SendAsync(Get("/secure/admin", token));

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    [Fact]
    public async Task Secure_WithTamperedToken_Returns401()
    {
        using var factory = NewApp();
        var client = Client(factory);
        var token = await AccessTokenFor(client, "admin", "password");

        var resp = await client.SendAsync(Get("/secure", Tamper(token)));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Secure_WithExpiredToken_Returns401()
    {
        using var factory = NewApp();
        var client = Client(factory);
        var settings = factory.Services.GetRequiredService<JwtSettings>();

        var resp = await client.SendAsync(Get("/secure", ExpiredAdminToken(settings)));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    // ---------- refresh + rotation ----------

    [Fact]
    public async Task Refresh_WithValidCookie_ReturnsNewAccessToken()
    {
        using var factory = NewApp();
        var client = Client(factory);
        var login = await client.PostAsJsonAsync("/auth/login", new LoginRequest("admin", "password"));
        var cookie = RefreshValue(login);

        var resp = await client.SendAsync(Refresh(cookie));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var body = await resp.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.False(string.IsNullOrWhiteSpace(body?.Token));
        Assert.False(string.IsNullOrEmpty(RefreshValue(resp)));            // a fresh cookie is issued
        Assert.NotEqual(cookie, RefreshValue(resp));                       // rotation: the value changes
    }

    [Fact]
    public async Task Refresh_RotatedToken_ReplayIsRejected()
    {
        using var factory = NewApp();
        var client = Client(factory);
        var login = await client.PostAsJsonAsync("/auth/login", new LoginRequest("admin", "password"));
        var first = RefreshValue(login);

        var ok = await client.SendAsync(Refresh(first));                   // consumes `first`, issues `second`
        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);

        var replay = await client.SendAsync(Refresh(first));               // re-presenting the used token

        Assert.Equal(HttpStatusCode.Unauthorized, replay.StatusCode);      // reuse detection
    }

    [Fact]
    public async Task Refresh_Reuse_RevokesTheWholeFamily()
    {
        using var factory = NewApp();
        var client = Client(factory);
        var login = await client.PostAsJsonAsync("/auth/login", new LoginRequest("admin", "password"));
        var first = RefreshValue(login);

        var rotated = await client.SendAsync(Refresh(first));              // first → consumed, second issued
        var second = RefreshValue(rotated);
        Assert.Equal(HttpStatusCode.OK, rotated.StatusCode);
        Assert.False(string.IsNullOrEmpty(second));

        var replay = await client.SendAsync(Refresh(first));              // reuse of `first` trips detection
        Assert.Equal(HttpStatusCode.Unauthorized, replay.StatusCode);

        var afterReuse = await client.SendAsync(Refresh(second));         // the live `second` must die too
        Assert.Equal(HttpStatusCode.Unauthorized, afterReuse.StatusCode); // proves RevokeAllFor, not just single-use
    }

    [Fact]
    public async Task Refresh_WithoutCookie_Returns401()
    {
        using var factory = NewApp();
        var client = Client(factory);

        var resp = await client.PostAsync("/auth/refresh", null);

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Logout_RevokesRefreshToken()
    {
        using var factory = NewApp();
        var client = Client(factory);
        var login = await client.PostAsJsonAsync("/auth/login", new LoginRequest("admin", "password"));
        var cookie = RefreshValue(login);

        await client.SendAsync(WithCookie(new HttpRequestMessage(HttpMethod.Post, "/auth/logout"), cookie));
        var afterLogout = await client.SendAsync(Refresh(cookie));

        Assert.Equal(HttpStatusCode.Unauthorized, afterLogout.StatusCode);
    }

    // ---------- helpers ----------

    private static async Task<string> AccessTokenFor(HttpClient client, string username, string password)
    {
        var login = await client.PostAsJsonAsync("/auth/login", new LoginRequest(username, password));
        var body = await login.Content.ReadFromJsonAsync<TokenResponse>();
        return body!.Token;
    }

    private static HttpRequestMessage Get(string path, string? bearer = null)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, path);
        if (bearer is not null)
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        return req;
    }

    private static HttpRequestMessage Refresh(string? cookieValue) =>
        WithCookie(new HttpRequestMessage(HttpMethod.Post, "/auth/refresh"), cookieValue);

    private static HttpRequestMessage WithCookie(HttpRequestMessage req, string? refreshValue)
    {
        if (refreshValue is not null)
            req.Headers.Add("Cookie", $"refresh_token={refreshValue}");
        return req;
    }

    private static string? RefreshValue(HttpResponseMessage resp)
    {
        if (!resp.Headers.TryGetValues("Set-Cookie", out var cookies)) return null;
        var c = cookies.FirstOrDefault(x => x.StartsWith("refresh_token="));
        if (c is null) return null;
        var value = c.Split(';')[0]["refresh_token=".Length..];
        return string.IsNullOrEmpty(value) ? null : value;
    }

    private static string ExpiredAdminToken(JwtSettings settings)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var issued = DateTime.UtcNow.AddMinutes(-5);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "admin"),
            new Claim(TokenService.RoleClaim, "Admin"),
        };
        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            notBefore: issued,
            expires: issued.AddMinutes(1),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string Tamper(string token)
    {
        var parts = token.Split('.');
        var payload = parts[1].ToCharArray();
        payload[0] = payload[0] == 'A' ? 'B' : 'A';
        parts[1] = new string(payload);
        return string.Join('.', parts);
    }
}
