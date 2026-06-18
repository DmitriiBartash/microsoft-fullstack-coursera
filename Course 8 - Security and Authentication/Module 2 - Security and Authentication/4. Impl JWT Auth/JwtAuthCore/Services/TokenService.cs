using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtAuthCore.Models;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthCore.Services;

/// <summary>
/// Issues signed JWTs and builds the matching validation parameters, so the issuing side
/// and the validating middleware always agree on key, issuer and audience.
/// </summary>
public class TokenService
{
    private readonly JwtSettings _settings;

    public TokenService(JwtSettings settings) => _settings = settings;

    public string GenerateToken(string username) =>
        GenerateToken(new User { Username = username }, lifetime: null);

    /// <summary>
    /// Issues a signed HS256 token carrying the user's subject and role, expiring after
    /// <paramref name="lifetime"/> or the configured default.
    /// </summary>
    public string GenerateToken(User user, TimeSpan? lifetime = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("role", user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: now.Add(lifetime ?? TimeSpan.FromMinutes(_settings.ExpiryMinutes)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>The single source of truth for token validation, reused by the API and the Studio.</summary>
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
        RoleClaimType = "role"
    };
}
