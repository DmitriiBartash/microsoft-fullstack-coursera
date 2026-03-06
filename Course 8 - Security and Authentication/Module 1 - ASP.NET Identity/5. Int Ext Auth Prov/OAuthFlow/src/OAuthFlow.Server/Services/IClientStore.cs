using OAuthFlow.Server.Domain.Entities;

namespace OAuthFlow.Server.Services;

public interface IClientStore
{
    RegisteredClient? FindById(string clientId);
    bool ValidateCredentials(string clientId, string clientSecret);
}
