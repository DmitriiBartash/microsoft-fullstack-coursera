using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OAuthFlow.Client.Models;

namespace OAuthFlow.Client.Pages;

public class DashboardModel : PageModel
{
    public UserInfoResponse? UserInfo { get; set; }
    public string? AccessToken { get; set; }
    public string? JwtHeader { get; set; }
    public string? JwtPayload { get; set; }
    public string? JwtSignature { get; set; }
    public List<OAuthFlowStep> FlowSteps { get; set; } = [];

    public IActionResult OnGet()
    {
        var userInfoJson = HttpContext.Session.GetString("user_info");
        AccessToken = HttpContext.Session.GetString("access_token");
        var stepsJson = HttpContext.Session.GetString("flow_steps");

        if (userInfoJson is null || AccessToken is null)
            return RedirectToPage("/Index");

        UserInfo = JsonSerializer.Deserialize<UserInfoResponse>(userInfoJson);
        FlowSteps = stepsJson is not null
            ? JsonSerializer.Deserialize<List<OAuthFlowStep>>(stepsJson) ?? []
            : [];

        DecodeJwt(AccessToken);

        return Page();
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Clear();
        return RedirectToPage("/Index");
    }

    private void DecodeJwt(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var headerJson = JsonSerializer.Serialize(
                JsonSerializer.Deserialize<JsonElement>(jwt.Header.SerializeToJson()),
                new JsonSerializerOptions { WriteIndented = true });

            var payloadJson = JsonSerializer.Serialize(
                JsonSerializer.Deserialize<JsonElement>(jwt.Payload.SerializeToJson()),
                new JsonSerializerOptions { WriteIndented = true });

            JwtHeader = headerJson;
            JwtPayload = payloadJson;

            var parts = token.Split('.');
            JwtSignature = parts.Length == 3 ? parts[2] : "";
        }
        catch
        {
            JwtHeader = "Failed to decode";
            JwtPayload = "Failed to decode";
            JwtSignature = "";
        }
    }
}
