namespace JwtSecurityCore;

/// <summary>
/// JWT signing and lifetime configuration. Best practice splits the lifetimes: a short-lived
/// <see cref="AccessTokenMinutes"/> bearer token limits the blast radius of a leak, while a
/// longer-lived <see cref="RefreshTokenDays"/> token (kept in an HttpOnly cookie) silently renews it.
/// The <see cref="Key"/> is a development default only — supply it from an environment variable
/// (JwtSettings__Key) or a secret store in any real environment.
/// </summary>
public class JwtSettings
{
    public const string DevelopmentDefaultKey = "SuperSecretKeyForJwtTokenAuthorization123456789";

    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;

    /// <summary>
    /// Refuses a weak or default signing key outside Development, so every host that binds these settings
    /// enforces the same rule. The key is a secret: supply it from an environment variable
    /// (JwtSettings__Key) or a secret store before running anywhere but Development.
    /// </summary>
    public void EnsureStrongKey(bool isDevelopment)
    {
        if (isDevelopment) return;
        if (Key.Length < 32 || Key == DevelopmentDefaultKey)
            throw new InvalidOperationException(
                "Set a strong JwtSettings:Key (>= 32 bytes) from an environment variable or secret store before running outside Development.");
    }
}
