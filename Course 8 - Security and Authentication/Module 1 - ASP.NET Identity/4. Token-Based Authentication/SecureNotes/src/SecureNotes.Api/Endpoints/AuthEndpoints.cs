using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SecureNotes.Api.Contracts.Auth;
using SecureNotes.Api.Services;

namespace SecureNotes.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
        {
            var response = await authService.RegisterAsync(request);
            return Results.Ok(response);
        }).AllowAnonymous();

        group.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
        {
            var response = await authService.LoginAsync(request);
            return Results.Ok(response);
        }).AllowAnonymous();

        group.MapPost("/refresh", async (RefreshRequest request, IAuthService authService) =>
        {
            var response = await authService.RefreshTokenAsync(request);
            return Results.Ok(response);
        }).AllowAnonymous();

        group.MapPost("/logout", async (ClaimsPrincipal user, IAuthService authService) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (userId is not null)
                await authService.LogoutAsync(userId);

            return Results.Ok(new { message = "Logged out successfully." });
        }).RequireAuthorization();

        return group;
    }
}
