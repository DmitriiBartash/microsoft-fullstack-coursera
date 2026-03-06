using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OAuthFlow.Client.Models;
using OAuthFlow.Client.Services;

namespace OAuthFlow.Client.Pages;

public class CallbackModel(IOAuthService oAuthService) : PageModel
{
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string? code, string? state, string? error)
    {
        var steps = LoadSteps();

        if (!string.IsNullOrEmpty(error))
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }

        var savedState = HttpContext.Session.GetString("oauth_state");
        if (state != savedState)
        {
            steps.Add(new OAuthFlowStep
            {
                StepNumber = 2,
                Title = "State Validation Failed",
                Status = "failed",
                Timestamp = DateTime.UtcNow,
                RequestInfo = $"Expected state: {savedState}",
                ResponseInfo = $"Received state: {state}"
            });
            SaveSteps(steps);
            ErrorMessage = "State mismatch - possible CSRF attack.";
            return Page();
        }

        steps.Add(new OAuthFlowStep
        {
            StepNumber = 2,
            Title = "User Authenticated & Consent Granted",
            Status = "completed",
            Timestamp = DateTime.UtcNow,
            RequestInfo = "User approved the authorization request",
            ResponseInfo = "Authorization server redirected to callback"
        });

        steps.Add(new OAuthFlowStep
        {
            StepNumber = 3,
            Title = "Authorization Code Received",
            Status = "completed",
            Timestamp = DateTime.UtcNow,
            RequestInfo = $"Callback URL parameters:\n  code={code?[..Math.Min(code?.Length ?? 0, 8)]}...\n  state={state?[..Math.Min(state?.Length ?? 0, 8)]}...",
            ResponseInfo = $"Authorization code: {code}"
        });

        var (token, tokenReqInfo, tokenRespInfo) = await oAuthService.ExchangeCodeForTokenAsync(code!);
        if (token is null)
        {
            steps.Add(new OAuthFlowStep
            {
                StepNumber = 4,
                Title = "Token Exchange Failed",
                Status = "failed",
                Timestamp = DateTime.UtcNow,
                RequestInfo = tokenReqInfo,
                ResponseInfo = tokenRespInfo
            });
            SaveSteps(steps);
            ErrorMessage = "Failed to exchange authorization code for token.";
            return Page();
        }

        steps.Add(new OAuthFlowStep
        {
            StepNumber = 4,
            Title = "Access Token Received",
            Status = "completed",
            Timestamp = DateTime.UtcNow,
            RequestInfo = tokenReqInfo,
            ResponseInfo = tokenRespInfo
        });

        HttpContext.Session.SetString("access_token", token.AccessToken);

        var (userInfo, userInfoReqInfo, userInfoRespInfo) = await oAuthService.GetUserInfoAsync(token.AccessToken);
        if (userInfo is null)
        {
            steps.Add(new OAuthFlowStep
            {
                StepNumber = 5,
                Title = "User Info Request Failed",
                Status = "failed",
                Timestamp = DateTime.UtcNow,
                RequestInfo = userInfoReqInfo,
                ResponseInfo = userInfoRespInfo
            });
            SaveSteps(steps);
            ErrorMessage = "Failed to fetch user info.";
            return Page();
        }

        steps.Add(new OAuthFlowStep
        {
            StepNumber = 5,
            Title = "User Info Retrieved",
            Status = "completed",
            Timestamp = DateTime.UtcNow,
            RequestInfo = userInfoReqInfo,
            ResponseInfo = userInfoRespInfo
        });

        steps.Add(new OAuthFlowStep
        {
            StepNumber = 6,
            Title = "Dashboard Ready",
            Status = "completed",
            Timestamp = DateTime.UtcNow,
            RequestInfo = "All OAuth flow steps completed successfully",
            ResponseInfo = $"User: {userInfo.Name} ({userInfo.Email})"
        });

        HttpContext.Session.SetString("user_info", JsonSerializer.Serialize(userInfo));
        SaveSteps(steps);

        return RedirectToPage("/Dashboard");
    }

    private List<OAuthFlowStep> LoadSteps()
    {
        var json = HttpContext.Session.GetString("flow_steps");
        return json is not null ? JsonSerializer.Deserialize<List<OAuthFlowStep>>(json) ?? [] : [];
    }

    private void SaveSteps(List<OAuthFlowStep> steps)
    {
        HttpContext.Session.SetString("flow_steps", JsonSerializer.Serialize(steps));
    }
}
