using System.Text.Json;
using Microsoft.Extensions.Options;
using OAuthFlow.Client.Models;

namespace OAuthFlow.Client.Services;

public class OAuthService(HttpClient httpClient, IOptions<OAuthSettings> settings) : IOAuthService
{
    private readonly OAuthSettings _settings = settings.Value;

    public async Task<(TokenResponse? Token, string RequestInfo, string ResponseInfo)> ExchangeCodeForTokenAsync(string code)
    {
        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = _settings.RedirectUri,
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret
        };

        var requestInfo = $"POST {_settings.TokenEndpoint}\n" +
                          $"Content-Type: application/x-www-form-urlencoded\n\n" +
                          $"grant_type=authorization_code&code={code}&redirect_uri={_settings.RedirectUri}&client_id={_settings.ClientId}&client_secret=***";

        using var content = new FormUrlEncodedContent(formData);
        var response = await httpClient.PostAsync(_settings.TokenEndpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        var responseInfo = $"HTTP {(int)response.StatusCode} {response.StatusCode}\n" +
                           $"Content-Type: application/json\n\n{responseBody}";

        if (!response.IsSuccessStatusCode)
            return (null, requestInfo, responseInfo);

        var token = JsonSerializer.Deserialize<TokenResponse>(responseBody);
        return (token, requestInfo, responseInfo);
    }

    public async Task<(UserInfoResponse? UserInfo, string RequestInfo, string ResponseInfo)> GetUserInfoAsync(string accessToken)
    {
        var requestInfo = $"GET {_settings.UserInfoEndpoint}\nAuthorization: Bearer {accessToken[..20]}...";

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.GetAsync(_settings.UserInfoEndpoint);
        var responseBody = await response.Content.ReadAsStringAsync();

        var responseInfo = $"HTTP {(int)response.StatusCode} {response.StatusCode}\n" +
                           $"Content-Type: application/json\n\n{responseBody}";

        if (!response.IsSuccessStatusCode)
            return (null, requestInfo, responseInfo);

        var userInfo = JsonSerializer.Deserialize<UserInfoResponse>(responseBody);
        return (userInfo, requestInfo, responseInfo);
    }
}
