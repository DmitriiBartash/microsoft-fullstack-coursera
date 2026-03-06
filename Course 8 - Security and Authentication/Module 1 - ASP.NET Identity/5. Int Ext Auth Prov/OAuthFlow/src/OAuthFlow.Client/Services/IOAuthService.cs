using OAuthFlow.Client.Models;

namespace OAuthFlow.Client.Services;

public interface IOAuthService
{
    Task<(TokenResponse? Token, string RequestInfo, string ResponseInfo)> ExchangeCodeForTokenAsync(string code);
    Task<(UserInfoResponse? UserInfo, string RequestInfo, string ResponseInfo)> GetUserInfoAsync(string accessToken);
}
