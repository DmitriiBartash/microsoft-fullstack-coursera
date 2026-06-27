using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtSecurityCore.Models;
using Microsoft.IdentityModel.Tokens;

namespace JwtSecurityCore.Services;

/// <summary>
/// Issues signed access tokens and builds the matching validation parameters, so the issuing side and
/// the validating middleware always agree on key, issuer, audience, lifetime and the role claim type.
/// Access tokens are deliberately short-lived; renewal is handled by <see cref="RefreshTokenStore"/>.
/// </summary>
public class TokenService
{
    public const string RoleClaim = "role";

    private readonly JwtSettings _settings;

    public TokenService(JwtSettings settings) => _settings = settings;

    /// <summary>
    /// Issues a signed HS256 access token carrying the user's subject and one role claim per role,
    /// expiring after <paramref name="lifetime"/> or the configured <c>AccessTokenMinutes</c>.
    /// </summary>
    public string GenerateToken(User user, TimeSpan? lifetime = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(user.Roles.Select(role => new Claim(RoleClaim, role)));

        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: now.Add(lifetime ?? TimeSpan.FromMinutes(_settings.AccessTokenMinutes)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>The single source of truth for token validation, reused by the API, the tests and the Studio.</summary>
    public TokenValidationParameters BuildValidationParameters() =>
        BuildValidationParameters(_settings);

    /// <inheritdoc cref="BuildValidationParameters()"/>
    public static TokenValidationParameters BuildValidationParameters(JwtSettings settings) => new()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
        ValidateIssuer = true,
        ValidIssuer = settings.Issuer,
        ValidateAudience = true,
        ValidAudience = settings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = JwtRegisteredClaimNames.Sub,
        RoleClaimType = RoleClaim
    };
}
