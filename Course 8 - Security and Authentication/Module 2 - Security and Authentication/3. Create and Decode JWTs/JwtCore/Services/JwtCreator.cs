using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtCore.Services;

public class JwtCreator
{
    public string Create(string subject, string name, string role, TimeSpan lifetime, string? secret = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? JwtConfig.DefaultSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, subject),
            new Claim(JwtRegisteredClaimNames.Name, name),
            new Claim("role", role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: JwtConfig.Issuer,
            audience: JwtConfig.Audience,
            claims: claims,
            notBefore: now,
            expires: now.Add(lifetime),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
