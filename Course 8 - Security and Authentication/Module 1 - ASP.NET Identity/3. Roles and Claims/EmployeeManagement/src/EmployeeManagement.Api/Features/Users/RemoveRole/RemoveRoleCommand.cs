using EmployeeManagement.Api.Common.Authorization;
using EmployeeManagement.Api.Common.Models;
using EmployeeManagement.Api.Domain.Entities;
using EmployeeManagement.Api.Features.Users.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Api.Features.Users.RemoveRole;

public record RemoveRoleCommand(string UserId, string Role) : IRequest<Result<UserResponse>>;

public class RemoveRoleHandler : IRequestHandler<RemoveRoleCommand, Result<UserResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RemoveRoleHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserResponse>> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) return Result<UserResponse>.Failure("User not found", 404);

        if (!await _userManager.IsInRoleAsync(user, request.Role))
            return Result<UserResponse>.Failure($"User doesn't have role '{request.Role}'");

        await _userManager.RemoveFromRoleAsync(user, request.Role);

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        var claimsDict = claims
            .Where(c => c.Type is CustomClaimTypes.Department or CustomClaimTypes.AccessLevel)
            .ToDictionary(c => c.Type, c => c.Value);

        return Result<UserResponse>.Success(
            new UserResponse(user.Id, user.Email!, user.FirstName, user.LastName, roles, claimsDict));
    }
}

public static class RemoveRoleEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/users/{userId}/roles/{role}", async (string userId, string role, IMediator mediator) =>
        {
            var result = await mediator.Send(new RemoveRoleCommand(userId, role));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error, statusCode: result.StatusCode);
        }).RequireAuthorization(PolicyNames.AdminOnly);
    }
}
