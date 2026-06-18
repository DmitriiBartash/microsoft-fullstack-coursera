using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using JwtAuthCore.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace JwtAuthDemo.Tests;

// Drives the real pipeline (auth middleware + [Authorize]) over an in-memory TestServer — no sockets.
public class AuthFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthFlowTests(WebApplicationFactory<Program> factory) => _factory = factory;

    private async Task<string> LoginAndGetTokenAsync(HttpClient client)
    {
        var login = await client.PostAsJsonAsync("/login", new LoginRequest("admin", "password"));
        var body = await login.Content.ReadFromJsonAsync<TokenResponse>();
        return body!.Token;
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var client = _factory.CreateClient();

        var resp = await client.PostAsJsonAsync("/login", new LoginRequest("admin", "password"));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var body = await resp.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.False(string.IsNullOrWhiteSpace(body?.Token));
    }

    [Fact]
    public async Task Login_WithBadCredentials_Returns401()
    {
        var client = _factory.CreateClient();

        var resp = await client.PostAsJsonAsync("/login", new LoginRequest("admin", "WRONG"));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task SecureData_WithValidToken_Returns200()
    {
        var client = _factory.CreateClient();
        var token = await LoginAndGetTokenAsync(client);

        var req = new HttpRequestMessage(HttpMethod.Get, "/secure-data");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var resp = await client.SendAsync(req);

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var body = await resp.Content.ReadFromJsonAsync<MessageResponse>();
        Assert.Contains("secure endpoint", body!.Message);
    }

    [Fact]
    public async Task SecureData_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var resp = await client.GetAsync("/secure-data");

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task SecureData_WithTamperedToken_Returns401()
    {
        var client = _factory.CreateClient();
        var token = await LoginAndGetTokenAsync(client);

        // Flip a character in the payload segment → the signature no longer matches the content.
        var parts = token.Split('.');
        var payload = parts[1].ToCharArray();
        payload[0] = payload[0] == 'A' ? 'B' : 'A';
        parts[1] = new string(payload);
        var tampered = string.Join('.', parts);

        var req = new HttpRequestMessage(HttpMethod.Get, "/secure-data");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tampered);
        var resp = await client.SendAsync(req);

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }
}
