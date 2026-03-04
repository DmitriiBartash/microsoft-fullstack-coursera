using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureNotes.Api.Contracts.Auth;
using SecureNotes.Api.Domain.Entities;
using SecureNotes.Api.Infrastructure.Data;

namespace SecureNotes.Api.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IConfiguration configuration,
    ApplicationDbContext dbContext) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new InvalidOperationException("User with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshRequest request)
    {
        var principal = tokenService.ValidateExpiredToken(request.AccessToken)
            ?? throw new UnauthorizedAccessException("Invalid access token.");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new UnauthorizedAccessException("Invalid token claims.");

        var user = await dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new UnauthorizedAccessException("User not found.");

        var existingToken = user.RefreshTokens
            .FirstOrDefault(rt => rt.Token == request.RefreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);

        if (existingToken is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        // Refresh token rotation
        existingToken.IsRevoked = true;

        return await GenerateAuthResponseAsync(user);
    }

    public async Task LogoutAsync(string userId)
    {
        var user = await dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return;

        foreach (var token in user.RefreshTokens.Where(rt => !rt.IsRevoked))
            token.IsRevoked = true;

        await dbContext.SaveChangesAsync();
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenDays = int.Parse(configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken);

        return new AuthResponse(accessToken, refreshToken, jwt.ValidTo);
    }
}
