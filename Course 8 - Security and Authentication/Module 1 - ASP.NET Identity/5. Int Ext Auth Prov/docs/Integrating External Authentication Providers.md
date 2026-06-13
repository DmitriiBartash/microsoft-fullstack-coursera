# Integrating External Authentication Providers

**Course 8 — Security and Authentication** · Module 1 · Lesson 5 · `You Try It!`

> Simulate an end-to-end **OAuth 2.0 Authorization Code** flow on your own machine.
> You build two ASP.NET Core apps — an **OAuth Server** (authorization server that issues
> codes and tokens) and an **OAuth Client** (the consumer app) — then watch an
> authorization code get exchanged for a signed access token.

---

## 🎯 Objective

Implement external authentication providers in ASP.NET Identity using **OAuth 2.0** by
standing up a local authorization server and a client app. By the end you will trigger the
`/authorize` → code → `/token` exchange yourself and inspect the access token the client
receives — making each moving part of the OAuth process concrete.

---

## 🗂️ What you will build

Two cooperating ASP.NET Core projects:

| Project        | Type             | Role                                                                    |
| -------------- | ---------------- | ----------------------------------------------------------------------- |
| `OAuthServer`  | Web API          | Authorization server — `/authorize` issues a code, `/token` issues a JWT |
| `OAuthClient`  | Razor Pages app  | Consumer — sends the user to `/authorize`, then swaps the code for a token |

**Flow:** `Client → Server /authorize → code → Client /callback → Server /token → access token`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (or any editor)
- NuGet package for token generation: `System.IdentityModel.Tokens.Jwt`
- Two free local ports (the examples use `5001` for the server and `5002` for the client)

---

## 🛠️ Steps

### Step 1 — Prepare the OAuth Server

You'll create the OAuth server, which simulates an external authorization server that issues
authorization codes and tokens.

Scaffold the Web API project and add the JWT library:

```bash
dotnet new webapi -o OAuthServer
cd OAuthServer
dotnet add package System.IdentityModel.Tokens.Jwt
```

Pin the listening port so the client knows where to find it — add this to
`OAuthServer/Properties/launchSettings.json` (or pass `--urls`):

```bash
dotnet run --urls "http://localhost:5001"
```

Create a controller named **`OAuthController.cs`** in a `Controllers` folder. It exposes the
two endpoints called out in the lab:

- **`/authorize`** — simulates login and returns an authorization code.
- **`/token`** — exchanges the authorization code for an access token.

```csharp
// Controllers/OAuthController.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace OAuthServer.Controllers;

[ApiController]
[Route("[action]")] // exposes /authorize and /token at the root
public class OAuthController : ControllerBase
{
    // Demo only: a real server signs with a secret kept out of source control.
    private const string SecretKey = "super-secret-demo-signing-key-change-me-1234567890";

    // In-memory store of issued authorization codes -> the user they belong to.
    private static readonly Dictionary<string, string> IssuedCodes = new();

    // GET /authorize?client_id=...&redirect_uri=...
    // Simulates the login/consent screen and redirects back with a one-time code.
    [HttpGet]
    public IActionResult Authorize([FromQuery] string redirect_uri)
    {
        // A real server authenticates the user here. We assume "demo-user" consented.
        var code = Guid.NewGuid().ToString("N");
        IssuedCodes[code] = "demo-user";

        var target = $"{redirect_uri}?code={code}";
        return Redirect(target);
    }

    // POST /token  { "code": "..." }
    // Exchanges a valid authorization code for a signed access token.
    [HttpPost]
    public IActionResult Token([FromBody] TokenRequest request)
    {
        if (request is null || !IssuedCodes.TryGetValue(request.Code, out var user))
            return BadRequest(new { error = "invalid_grant" });

        IssuedCodes.Remove(request.Code); // codes are single-use

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "OAuthServer",
            audience: "OAuthClient",
            claims: new[]
            {
                new Claim(ClaimTypes.Name, user),
                new Claim(JwtRegisteredClaimNames.Sub, user)
            },
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { access_token = accessToken, token_type = "Bearer", expires_in = 900 });
    }

    public record TokenRequest(string Code);
}
```

Make sure `Program.cs` maps controllers (the `webapi` template already adds Swagger):

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.Run();
```

### Step 2 — Prepare the OAuth Client

You'll create the OAuth client, which simulates a client app requesting an authorization code
and token.

Scaffold the Razor Pages app:

```bash
dotnet new webapp -o OAuthClient
cd OAuthClient
```

Add a **"Login with OAuth"** button to the homepage that redirects users to the OAuth Server's
`/authorize` endpoint, passing the client's callback as `redirect_uri`:

```html
<!-- Pages/Index.cshtml -->
@page
@model IndexModel

<h1>OAuth Client</h1>

<a class="btn btn-primary"
   href="http://localhost:5001/authorize?client_id=demo-client&redirect_uri=http://localhost:5002/callback">
   Login with OAuth
</a>

@if (!string.IsNullOrEmpty(Model.AccessToken))
{
    <h3>Access token received:</h3>
    <pre style="white-space:pre-wrap">@Model.AccessToken</pre>
}
```

Add a **callback endpoint** to handle the authorization code and exchange it for a token.
A minimal-API endpoint registered in `Program.cs` keeps the round-trip in one place:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseStaticFiles();
app.MapRazorPages();

// GET /callback?code=... — swaps the code for a token, then shows the home page.
app.MapGet("/callback", async (string code, IHttpClientFactory httpFactory) =>
{
    var http = httpFactory.CreateClient();
    var response = await http.PostAsJsonAsync(
        "http://localhost:5001/token", new { code });

    if (!response.IsSuccessStatusCode)
        return Results.BadRequest("Token exchange failed.");

    var payload = await response.Content.ReadFromJsonAsync<TokenResponse>();
    // Render the token on the home page (for the demo we pass it via query string).
    return Results.Redirect($"/?token={Uri.EscapeDataString(payload!.access_token)}");
});

app.Run();

record TokenResponse(string access_token, string token_type, int expires_in);
```

Read the returned token back into the page model so the homepage can display it:

```csharp
// Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    public string? AccessToken { get; private set; }

    public void OnGet(string? token) => AccessToken = token;
}
```

### Step 3 — Test the OAuth flow

Start **both** applications, each in its own terminal:

```bash
# Terminal 1 — authorization server
dotnet run --project OAuthServer --urls "http://localhost:5001"
```

```bash
# Terminal 2 — client app
dotnet run --project OAuthClient --urls "http://localhost:5002"
```

Then walk the flow:

- Open the OAuth Client app in your browser at `http://localhost:5002`.
- Click the **"Login with OAuth"** button and follow the redirect to the server.
- The server issues an authorization code and redirects back to `/callback`.
- The client exchanges that code at `/token` and **displays the access token** it received.

---

## ▶️ Expected result

Clicking **Login with OAuth** bounces the browser to the server's `/authorize`, back to the
client's `/callback` carrying a `code`, and finally lands on the home page showing a signed
**JWT access token** (a `Bearer` token valid for 15 minutes). Seeing that token proves the
authorization-code-for-token exchange completed end to end.

---

## ☑️ Definition of done

- [ ] `OAuthServer` Web API project created and `System.IdentityModel.Tokens.Jwt` added
- [ ] `OAuthController.cs` exposes `/authorize` (returns a code) and `/token` (returns a JWT)
- [ ] `OAuthClient` Razor Pages project created with a **Login with OAuth** button
- [ ] A `/callback` endpoint exchanges the code for a token via the server's `/token`
- [ ] Both apps run (`:5001` server, `:5002` client) and the client displays the access token

---

## 🔑 Key concepts

- **Authorization Code grant** — the client never sees the user's credentials; it receives a
  short-lived *code* and swaps it server-to-server for a token, which keeps the secret exchange
  off the browser.
- **Two distinct roles** — the *authorization server* (`OAuthServer`) authenticates and issues
  tokens; the *client* (`OAuthClient`) consumes them. External providers like Google or GitHub
  simply play the authorization-server role.
- **`redirect_uri` is the contract** — the server only sends codes back to a pre-agreed callback,
  which is what ties the issued code to the legitimate client.
- **Codes are single-use and short-lived** — exchanging a code consumes it; this limits the blast
  radius if a code leaks. The resulting **JWT is signed (HS256)** so the client can trust its claims.
