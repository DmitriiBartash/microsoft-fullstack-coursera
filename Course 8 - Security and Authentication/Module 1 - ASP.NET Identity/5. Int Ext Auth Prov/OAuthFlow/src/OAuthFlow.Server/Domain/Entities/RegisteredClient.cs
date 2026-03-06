namespace OAuthFlow.Server.Domain.Entities;

public class RegisteredClient
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string ClientName { get; init; }
    public required List<string> RedirectUris { get; init; }
}
