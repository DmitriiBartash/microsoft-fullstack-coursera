using OAuthFlow.Server.Domain.Entities;

namespace OAuthFlow.Server.Services;

public interface IUserStore
{
    OAuthUser? Authenticate(string username, string password);
    OAuthUser? FindById(string userId);
}
