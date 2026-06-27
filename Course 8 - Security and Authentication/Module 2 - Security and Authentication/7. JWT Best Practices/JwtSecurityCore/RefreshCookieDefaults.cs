namespace JwtSecurityCore;

/// <summary>
/// Single source of truth for the refresh-token cookie's name and path, shared by every host so the
/// cookie a server sets is the cookie the client reads back. Scoping the path to <see cref="Path"/>
/// means the cookie is only ever sent to the auth endpoints, not to protected resources.
/// </summary>
public static class RefreshCookieDefaults
{
    public const string Name = "refresh_token";
    public const string Path = "/auth";
}
