using System.Security.Claims;
using SecureNotes.Api.Domain.Entities;

namespace SecureNotes.Api.Services;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateExpiredToken(string token);
}
