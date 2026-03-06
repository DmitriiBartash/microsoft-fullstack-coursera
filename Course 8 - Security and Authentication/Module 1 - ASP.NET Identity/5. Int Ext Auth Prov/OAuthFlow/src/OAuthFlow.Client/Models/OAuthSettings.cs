namespace OAuthFlow.Client.Models;

public class OAuthSettings
{
    public required string AuthorizationEndpoint { get; init; }
    public required string TokenEndpoint { get; init; }
    public required string UserInfoEndpoint { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string RedirectUri { get; init; }
    public required string Scopes { get; init; }
}
