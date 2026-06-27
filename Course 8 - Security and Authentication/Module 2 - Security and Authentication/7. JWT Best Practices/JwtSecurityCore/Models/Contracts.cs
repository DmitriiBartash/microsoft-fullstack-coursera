namespace JwtSecurityCore.Models;

public record LoginRequest(string Username, string Password);

/// <summary>
/// The login/refresh response. The access token travels in the body (the client holds it in memory
/// and sends it as a Bearer header); <see cref="ExpiresInSeconds"/> lets the client schedule a refresh.
/// In the hardened flow the refresh token is NOT here — it is set as an HttpOnly cookie the browser
/// cannot expose to JavaScript. <see cref="RefreshToken"/> is populated only by the deliberately weak
/// flow, to show what leaking it into a script-readable place looks like.
/// </summary>
public record TokenResponse(string Token, int ExpiresInSeconds, string? RefreshToken = null);

public record MessageResponse(string Message);
