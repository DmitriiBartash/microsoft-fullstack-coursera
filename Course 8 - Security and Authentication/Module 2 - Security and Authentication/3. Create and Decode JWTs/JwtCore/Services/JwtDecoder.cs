using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using JwtCore.Models;

namespace JwtCore.Services;

// Validate() enforces the signature and throws on failure; Inspect() always decodes and reports validity.
public class JwtDecoder
{
    private static readonly JwtSecurityTokenHandler Handler = new() { MapInboundClaims = false };

    public ClaimsPrincipal Validate(string token, string? secret = null)
        => Handler.ValidateToken(token, BuildParameters(secret), out _);

    public InspectionResult Inspect(string token, string? secret = null)
    {
        JwtSecurityToken jwt;
        try
        {
            jwt = Handler.ReadJwtToken(token);
        }
        catch (Exception ex)
        {
            return new InspectionResult { Status = "malformed", Message = "Not a valid JWT: " + ex.Message };
        }

        var segments = token.Split('.');
        var parts = new TokenParts(
            Prettify(jwt.Header.SerializeToJson()),
            Prettify(jwt.Payload.SerializeToJson()),
            segments.Length > 2 ? segments[2] : string.Empty);
        var claims = jwt.Payload.Claims.Select(c => new ClaimItem(c.Type, c.Value)).ToList();

        try
        {
            Validate(token, secret);
            return new InspectionResult
            {
                IsValid = true,
                IsReadable = true,
                Status = "valid",
                Message = "Signature valid — token is authentic and unexpired.",
                Parts = parts,
                Claims = claims
            };
        }
        catch (SecurityTokenExpiredException)
        {
            return Failed(parts, claims, "expired", "Token has expired — its exp time is in the past.");
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            return Failed(parts, claims, "bad-signature", "Signature mismatch — wrong key or the payload was tampered with.");
        }
        catch (SecurityTokenException ex)
        {
            return Failed(parts, claims, "invalid", ex.Message);
        }
    }

    private static InspectionResult Failed(TokenParts parts, IReadOnlyList<ClaimItem> claims, string status, string message)
        => new() { IsReadable = true, Status = status, Message = message, Parts = parts, Claims = claims };

    private static TokenValidationParameters BuildParameters(string? secret) => new()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? JwtConfig.DefaultSecret)),
        ValidateIssuer = true,
        ValidIssuer = JwtConfig.Issuer,
        ValidateAudience = true,
        ValidAudience = JwtConfig.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    private static string Prettify(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
    }
}
