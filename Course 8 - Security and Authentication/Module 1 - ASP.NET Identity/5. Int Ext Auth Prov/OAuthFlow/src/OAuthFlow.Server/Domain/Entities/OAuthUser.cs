namespace OAuthFlow.Server.Domain.Entities;

public class OAuthUser
{
    public required string UserId { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string DisplayName { get; init; }
    public required string Email { get; init; }
}
