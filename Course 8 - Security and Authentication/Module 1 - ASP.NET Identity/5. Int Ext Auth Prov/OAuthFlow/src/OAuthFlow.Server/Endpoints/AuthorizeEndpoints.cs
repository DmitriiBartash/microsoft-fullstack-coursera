using System.Security.Cryptography;
using OAuthFlow.Server.Domain.Entities;
using OAuthFlow.Server.Services;

namespace OAuthFlow.Server.Endpoints;

public static class AuthorizeEndpoints
{
    public static RouteGroupBuilder MapAuthorizeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Authorization");

        group.MapGet("/authorize", (
            string? response_type,
            string? client_id,
            string? redirect_uri,
            string? state,
            string? scope,
            IClientStore clientStore) =>
        {
            if (response_type != "code")
                return Results.Content(BuildErrorHtml("Unsupported response_type. Only 'code' is supported."), "text/html");

            if (string.IsNullOrEmpty(client_id) || string.IsNullOrEmpty(redirect_uri))
                return Results.Content(BuildErrorHtml("Missing required parameters: client_id and redirect_uri."), "text/html");

            var client = clientStore.FindById(client_id);
            if (client is null)
                return Results.Content(BuildErrorHtml("Unknown client: " + client_id), "text/html");

            if (!client.RedirectUris.Contains(redirect_uri))
                return Results.Content(BuildErrorHtml("Invalid redirect_uri for this client."), "text/html");

            return Results.Content(BuildConsentHtml(client, redirect_uri, state ?? "", scope ?? "openid", response_type, null), "text/html");
        }).AllowAnonymous();

        group.MapPost("/authorize", async (HttpContext context, IClientStore clientStore, IUserStore userStore, IAuthCodeStore authCodeStore) =>
        {
            var form = await context.Request.ReadFormAsync();
            var clientId = form["client_id"].ToString();
            var redirectUri = form["redirect_uri"].ToString();
            var state = form["state"].ToString();
            var scope = form["scope"].ToString();
            var responseType = form["response_type"].ToString();
            var username = form["username"].ToString();
            var password = form["password"].ToString();
            var action = form["action"].ToString();

            var client = clientStore.FindById(clientId);
            if (client is null || !client.RedirectUris.Contains(redirectUri))
                return Results.Content(BuildErrorHtml("Invalid client or redirect URI."), "text/html");

            if (action == "deny")
                return Results.Redirect(redirectUri + "?error=access_denied&state=" + Uri.EscapeDataString(state));

            var user = userStore.Authenticate(username, password);
            if (user is null)
                return Results.Content(BuildConsentHtml(client, redirectUri, state, scope, responseType, "Invalid username or password."), "text/html");

            var codeValue = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
            var authCode = new AuthorizationCode
            {
                Code = codeValue,
                ClientId = clientId,
                UserId = user.UserId,
                RedirectUri = redirectUri,
                Scope = scope,
                ExpiresAt = DateTime.UtcNow.AddSeconds(60)
            };
            authCodeStore.Store(authCode);

            return Results.Redirect(redirectUri + "?code=" + codeValue + "&state=" + Uri.EscapeDataString(state));
        }).AllowAnonymous().DisableAntiforgery();

        return group;
    }

    private static string BuildErrorHtml(string message)
    {
        var encoded = System.Net.WebUtility.HtmlEncode(message);
        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <title>Authorization Error</title>
                <style>
                    * { margin: 0; padding: 0; box-sizing: border-box; }
                    body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; background: #f0f2f5; display: flex; justify-content: center; align-items: center; min-height: 100vh; }
                    .error-card { background: #fff; border-radius: 12px; padding: 48px; max-width: 480px; width: 90%; box-shadow: 0 2px 12px rgba(0,0,0,0.08); text-align: center; }
                    .error-icon { font-size: 48px; margin-bottom: 16px; }
                    h1 { color: #dc3545; font-size: 24px; margin-bottom: 12px; }
                    p { color: #555; line-height: 1.6; }
                </style>
            </head>
            <body>
                <div class="error-card">
                    <div class="error-icon">&#9888;</div>
                    <h1>Authorization Error</h1>
                    <p>{{encoded}}</p>
                </div>
            </body>
            </html>
            """;
    }

    private static string BuildConsentHtml(RegisteredClient client, string redirectUri, string state, string scope, string responseType, string? error)
    {
        var scopes = scope.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var scopeListHtml = string.Join("", scopes.Select(s => "<li>" + System.Net.WebUtility.HtmlEncode(s) + "</li>"));
        var errorHtml = error is not null
            ? "<div class=\"error-msg\">" + System.Net.WebUtility.HtmlEncode(error) + "</div>"
            : "";
        var clientNameEncoded = System.Net.WebUtility.HtmlEncode(client.ClientName);
        var clientIdEncoded = System.Net.WebUtility.HtmlEncode(client.ClientId);
        var redirectUriEncoded = System.Net.WebUtility.HtmlEncode(redirectUri);
        var stateEncoded = System.Net.WebUtility.HtmlEncode(state);
        var scopeEncoded = System.Net.WebUtility.HtmlEncode(scope);
        var responseTypeEncoded = System.Net.WebUtility.HtmlEncode(responseType);

        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <title>Authorize - OAuthFlow Server</title>
                <style>
                    * { margin: 0; padding: 0; box-sizing: border-box; }
                    body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; background: #f0f2f5; display: flex; justify-content: center; align-items: center; min-height: 100vh; }
                    .consent-card { background: #fff; border-radius: 12px; padding: 40px; max-width: 440px; width: 90%; box-shadow: 0 2px 16px rgba(0,0,0,0.1); }
                    .server-badge { background: #6366f1; color: #fff; padding: 6px 14px; border-radius: 20px; font-size: 13px; font-weight: 600; display: inline-block; margin-bottom: 24px; }
                    h1 { font-size: 22px; color: #1a1a2e; margin-bottom: 8px; }
                    .client-name { color: #6366f1; font-weight: 600; }
                    .desc { color: #666; font-size: 14px; margin-bottom: 24px; line-height: 1.5; }
                    .scopes { background: #f8f9fa; border-radius: 8px; padding: 16px; margin-bottom: 24px; }
                    .scopes h3 { font-size: 13px; color: #888; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 8px; }
                    .scopes ul { list-style: none; }
                    .scopes li { padding: 6px 0; color: #333; font-size: 14px; }
                    .scopes li::before { content: "\2713"; color: #6366f1; font-weight: bold; margin-right: 8px; }
                    .form-group { margin-bottom: 16px; }
                    label { display: block; font-size: 13px; font-weight: 600; color: #444; margin-bottom: 6px; }
                    input[type="text"], input[type="password"] { width: 100%; padding: 10px 14px; border: 1px solid #ddd; border-radius: 8px; font-size: 14px; transition: border-color 0.2s; }
                    input[type="text"]:focus, input[type="password"]:focus { outline: none; border-color: #6366f1; box-shadow: 0 0 0 3px rgba(99,102,241,0.1); }
                    .actions { display: flex; gap: 12px; margin-top: 24px; }
                    .btn { flex: 1; padding: 12px; border: none; border-radius: 8px; font-size: 15px; font-weight: 600; cursor: pointer; transition: opacity 0.2s; }
                    .btn-approve { background: #6366f1; color: #fff; }
                    .btn-deny { background: #e9ecef; color: #555; }
                    .btn:hover { opacity: 0.9; }
                    .error-msg { background: #fee2e2; color: #dc3545; padding: 10px 14px; border-radius: 8px; font-size: 14px; margin-bottom: 16px; }
                </style>
            </head>
            <body>
                <div class="consent-card">
                    <div class="server-badge">OAuthFlow Server</div>
                    <h1>Authorization Request</h1>
                    <p class="desc"><span class="client-name">{{clientNameEncoded}}</span> requests access to your account</p>
                    {{errorHtml}}
                    <div class="scopes">
                        <h3>Requested Permissions</h3>
                        <ul>{{scopeListHtml}}</ul>
                    </div>
                    <form method="post" action="/authorize">
                        <div class="form-group">
                            <label for="username">Username</label>
                            <input type="text" id="username" name="username" required autocomplete="username" />
                        </div>
                        <div class="form-group">
                            <label for="password">Password</label>
                            <input type="password" id="password" name="password" required autocomplete="current-password" />
                        </div>
                        <input type="hidden" name="client_id" value="{{clientIdEncoded}}" />
                        <input type="hidden" name="redirect_uri" value="{{redirectUriEncoded}}" />
                        <input type="hidden" name="state" value="{{stateEncoded}}" />
                        <input type="hidden" name="scope" value="{{scopeEncoded}}" />
                        <input type="hidden" name="response_type" value="{{responseTypeEncoded}}" />
                        <div class="actions">
                            <button type="submit" name="action" value="deny" class="btn btn-deny" formnovalidate>Deny</button>
                            <button type="submit" name="action" value="approve" class="btn btn-approve">Approve</button>
                        </div>
                    </form>
                </div>
            </body>
            </html>
            """;
    }
}
