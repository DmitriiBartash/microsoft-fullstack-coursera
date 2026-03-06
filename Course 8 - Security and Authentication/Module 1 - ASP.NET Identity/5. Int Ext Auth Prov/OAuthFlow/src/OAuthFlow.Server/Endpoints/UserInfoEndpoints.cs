using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using OAuthFlow.Server.Contracts;

namespace OAuthFlow.Server.Endpoints;

public static class UserInfoEndpoints
{
    public static RouteGroupBuilder MapUserInfoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("UserInfo");

        group.MapGet("/userinfo", (ClaimsPrincipal user) =>
        {
            var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var name = user.FindFirstValue(JwtRegisteredClaimNames.Name)
                       ?? user.FindFirstValue(ClaimTypes.Name) ?? "";
            var email = user.FindFirstValue(JwtRegisteredClaimNames.Email)
                        ?? user.FindFirstValue(ClaimTypes.Email) ?? "";

            return Results.Ok(new UserInfoResponse
            {
                Sub = sub,
                Name = name,
                Email = email
            });
        }).RequireAuthorization();

        return group;
    }
}
