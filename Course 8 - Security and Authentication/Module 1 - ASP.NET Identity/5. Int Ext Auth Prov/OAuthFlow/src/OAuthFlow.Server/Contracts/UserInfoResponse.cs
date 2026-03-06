using System.Text.Json.Serialization;

namespace OAuthFlow.Server.Contracts;

public class UserInfoResponse
{
    [JsonPropertyName("sub")]
    public required string Sub { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("email")]
    public required string Email { get; init; }
}
