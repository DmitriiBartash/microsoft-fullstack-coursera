using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using OAuthFlow.Client.Models;

namespace OAuthFlow.Client.Pages;

public class IndexModel(IOptions<OAuthSettings> settings) : PageModel
{
    private readonly OAuthSettings _settings = settings.Value;

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        var state = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
        HttpContext.Session.SetString("oauth_state", state);

        HttpContext.Session.Remove("flow_steps");
        HttpContext.Session.Remove("access_token");
        HttpContext.Session.Remove("user_info");

        var steps = new List<OAuthFlowStep>
        {
            new()
            {
                StepNumber = 1,
                Title = "Redirect to Authorization Server",
                Status = "completed",
                Timestamp = DateTime.UtcNow,
                RequestInfo = $"GET {_settings.AuthorizationEndpoint}\n  response_type=code\n  client_id={_settings.ClientId}\n  redirect_uri={_settings.RedirectUri}\n  scope={_settings.Scopes}\n  state={state}",
                ResponseInfo = "HTTP 302 Redirect to consent page"
            }
        };

        HttpContext.Session.SetString("flow_steps", System.Text.Json.JsonSerializer.Serialize(steps));

        var authUrl = _settings.AuthorizationEndpoint +
            "?response_type=code" +
            "&client_id=" + Uri.EscapeDataString(_settings.ClientId) +
            "&redirect_uri=" + Uri.EscapeDataString(_settings.RedirectUri) +
            "&state=" + Uri.EscapeDataString(state) +
            "&scope=" + Uri.EscapeDataString(_settings.Scopes);

        return Redirect(authUrl);
    }
}
