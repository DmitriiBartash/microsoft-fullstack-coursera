namespace JwtCore;

public static class JwtConfig
{
    // Demo key only. HS256 needs >= 256 bits (>= 32 ASCII chars); load from config/secret store in production.
    public const string DefaultSecret = "jwt-lab-demo-signing-key-change-me-please-0123456789";

    public const string Issuer = "JwtLab";
    public const string Audience = "JwtLabUsers";
}
