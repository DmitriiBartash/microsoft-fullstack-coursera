namespace OAuthFlow.Server.Domain.Entities;

public class AuthorizationCode
{
    public required string Code { get; init; }
    public required string ClientId { get; init; }
    public required string UserId { get; init; }
    public required string RedirectUri { get; init; }
    public required string Scope { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public required DateTime ExpiresAt { get; init; }
}
