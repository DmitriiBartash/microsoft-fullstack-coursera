using OAuthFlow.Server.Domain.Entities;

namespace OAuthFlow.Server.Services;

public class ClientStore : IClientStore
{
    private readonly List<RegisteredClient> _clients;

    public ClientStore(IConfiguration configuration)
    {
        _clients = configuration.GetSection("OAuthClients").Get<List<RegisteredClient>>() ?? [];
    }

    public RegisteredClient? FindById(string clientId) =>
        _clients.FirstOrDefault(c => c.ClientId == clientId);

    public bool ValidateCredentials(string clientId, string clientSecret)
    {
        var client = FindById(clientId);
        return client is not null && client.ClientSecret == clientSecret;
    }
}
