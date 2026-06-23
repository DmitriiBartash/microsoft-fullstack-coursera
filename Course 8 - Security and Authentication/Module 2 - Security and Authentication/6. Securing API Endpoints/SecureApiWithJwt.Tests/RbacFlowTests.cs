using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SecureApiCore;
using SecureApiCore.Models;
using SecureApiCore.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SecureApiWithJwt.Tests;

// Drives the real pipeline (JwtBearer + [Authorize(Roles = ...)]) over an in-memory TestServer — no sockets.
public class RbacFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RbacFlowTests(WebApplicationFactory<Program> factory) => _factory = factory;

    private static async Task<string> TokenFor(HttpClient client, string username, string password)
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

    // ---------- login ----------

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var client = _factory.CreateClient();

        var resp = await client.PostAsJsonAsync("/auth/login", new LoginRequest("admin", "password"));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var body = await resp.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.False(string.IsNullOrWhiteSpace(body?.Token));
    }

    [Fact]
    public async Task Login_WithBadCredentials_Returns401()
    {
        var client = _factory.CreateClient();

        var resp = await client.PostAsJsonAsync("/auth/login", new LoginRequest("admin", "WRONG"));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    // ---------- public route (anonymous allowed) ----------

    [Fact]
    public async Task Public_WithoutToken_Returns200()
    {
        var client = _factory.CreateClient();

        var resp = await client.GetAsync("/values/public");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    // ---------- member route (Admin or User) ----------

    [Fact]
    public async Task UserArea_WithUserToken_Returns200()
    {
        var client = _factory.CreateClient();
        var token = await TokenFor(client, "alice", "pa55");

        var resp = await client.SendAsync(Get("/values/user", token));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task UserArea_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var resp = await client.SendAsync(Get("/values/user"));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    // ---------- admin route (Admin only) — the RBAC heart: 200 vs 401 vs 403 ----------

    [Fact]
    public async Task AdminArea_WithAdminToken_Returns200()
    {
        var client = _factory.CreateClient();
        var token = await TokenFor(client, "admin", "password");

        var resp = await client.SendAsync(Get("/values/admin", token));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task AdminArea_WithUserToken_Returns403()
    {
        var client = _factory.CreateClient();
        var token = await TokenFor(client, "alice", "pa55");   // alice is User, not Admin

        var resp = await client.SendAsync(Get("/values/admin", token));

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    [Fact]
    public async Task AdminArea_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var resp = await client.SendAsync(Get("/values/admin"));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task AdminArea_WithTamperedToken_Returns401()
    {
        var client = _factory.CreateClient();
        var token = await TokenFor(client, "admin", "password");

        var resp = await client.SendAsync(Get("/values/admin", Tamper(token)));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task AdminArea_WithExpiredToken_Returns401()
    {
        var client = _factory.CreateClient();

        // Mint a token whose lifetime is entirely in the past, signed with the API's real key.
        var settings = _factory.Services.GetRequiredService<JwtSettings>();
        var expired = ExpiredAdminToken(settings);

        var resp = await client.SendAsync(Get("/values/admin", expired));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    // Builds an Admin token whose lifetime is entirely in the past (valid signature, but expired).
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
            expires: issued.AddMinutes(1),   // expired ~4 minutes ago
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Flip a character in the payload segment → the signature no longer matches the content.
    private static string Tamper(string token)
    {
        var parts = token.Split('.');
        var payload = parts[1].ToCharArray();
        payload[0] = payload[0] == 'A' ? 'B' : 'A';
        parts[1] = new string(payload);
        return string.Join('.', parts);
    }
}
