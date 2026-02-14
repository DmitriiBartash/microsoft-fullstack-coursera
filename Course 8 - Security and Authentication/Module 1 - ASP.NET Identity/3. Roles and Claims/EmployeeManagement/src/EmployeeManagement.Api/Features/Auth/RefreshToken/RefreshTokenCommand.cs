using System.IdentityModel.Tokens.Jwt;
using System.Text;
using EmployeeManagement.Api.Common.Models;
using EmployeeManagement.Api.Domain.Entities;
using EmployeeManagement.Api.Features.Auth.Contracts;
using EmployeeManagement.Api.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Api.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<AuthResponse>>;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly IConfiguration _configuration;

    public RefreshTokenHandler(UserManager<ApplicationUser> userManager, TokenService tokenService, IConfiguration configuration)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
            return Result<AuthResponse>.Failure("Invalid access token", 401);

        var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (userId is null)
            return Result<AuthResponse>.Failure("Invalid access token", 401);

        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Result<AuthResponse>.Failure("User not found", 404);

        var existingToken = user.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken && t.IsActive);
        if (existingToken is null)
            return Result<AuthResponse>.Failure("Invalid refresh token", 401);

        existingToken.RevokedAt = DateTime.UtcNow;

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);

        var newAccessToken = _tokenService.GenerateAccessToken(user, roles, claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);

        return Result<AuthResponse>.Success(
            new AuthResponse(newAccessToken, newRefreshToken.Token, DateTime.UtcNow.AddMinutes(30)));
    }

    private System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!))
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}

public static class RefreshTokenEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/refresh", async (RefreshTokenCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error, statusCode: result.StatusCode);
        }).AllowAnonymous();
    }
}
