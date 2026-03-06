using OAuthFlow.Server.Domain.Entities;

namespace OAuthFlow.Server.Services;

public class UserStore : IUserStore
{
    private readonly List<OAuthUser> _users;

    public UserStore(IConfiguration configuration)
    {
        _users = configuration.GetSection("DemoUsers").Get<List<OAuthUser>>() ?? [];
    }

    public OAuthUser? Authenticate(string username, string password) =>
        _users.FirstOrDefault(u => u.Username == username && u.Password == password);

    public OAuthUser? FindById(string userId) =>
        _users.FirstOrDefault(u => u.UserId == userId);
}
