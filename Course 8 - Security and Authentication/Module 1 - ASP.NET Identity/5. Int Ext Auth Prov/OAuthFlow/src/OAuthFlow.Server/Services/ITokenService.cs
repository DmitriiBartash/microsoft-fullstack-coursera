using OAuthFlow.Server.Domain.Entities;

namespace OAuthFlow.Server.Services;

public interface ITokenService
{
    string GenerateAccessToken(OAuthUser user, string scope);
}
