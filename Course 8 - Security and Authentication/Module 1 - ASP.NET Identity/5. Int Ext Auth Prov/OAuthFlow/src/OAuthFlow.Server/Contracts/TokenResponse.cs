using System.Text.Json.Serialization;

namespace OAuthFlow.Server.Contracts;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "Bearer";

    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; init; }

    [JsonPropertyName("scope")]
    public required string Scope { get; init; }
}
