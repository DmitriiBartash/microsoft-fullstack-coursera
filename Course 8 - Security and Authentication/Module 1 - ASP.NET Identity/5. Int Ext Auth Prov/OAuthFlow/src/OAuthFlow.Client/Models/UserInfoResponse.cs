using System.Text.Json.Serialization;

namespace OAuthFlow.Client.Models;

public class UserInfoResponse
{
    [JsonPropertyName("sub")]
    public string Sub { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("email")]
    public string Email { get; set; } = "";
}
