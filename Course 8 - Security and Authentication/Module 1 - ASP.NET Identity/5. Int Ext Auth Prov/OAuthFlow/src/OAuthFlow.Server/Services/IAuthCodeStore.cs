using OAuthFlow.Server.Domain.Entities;

namespace OAuthFlow.Server.Services;

public interface IAuthCodeStore
{
    void Store(AuthorizationCode code);
    AuthorizationCode? RetrieveAndDelete(string code);
}
