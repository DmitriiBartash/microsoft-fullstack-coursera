using EmployeeManagement.Api.Common.Authorization;
using EmployeeManagement.Api.Common.Models;
using EmployeeManagement.Api.Domain.Entities;
using EmployeeManagement.Api.Features.Users.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Api.Features.Users.RemoveClaim;

public record RemoveClaimCommand(string UserId, string ClaimType) : IRequest<Result<UserResponse>>;

public class RemoveClaimHandler : IRequestHandler<RemoveClaimCommand, Result<UserResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RemoveClaimHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserResponse>> Handle(RemoveClaimCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) return Result<UserResponse>.Failure("User not found", 404);

        var claims = await _userManager.GetClaimsAsync(user);
        var claimToRemove = claims.FirstOrDefault(c => c.Type == request.ClaimType);

        if (claimToRemove is null)
            return Result<UserResponse>.Failure($"Claim '{request.ClaimType}' not found");

        await _userManager.RemoveClaimAsync(user, claimToRemove);

        var roles = await _userManager.GetRolesAsync(user);
        var updatedClaims = await _userManager.GetClaimsAsync(user);
        var claimsDict = updatedClaims
            .Where(c => c.Type is CustomClaimTypes.Department or CustomClaimTypes.AccessLevel)
            .ToDictionary(c => c.Type, c => c.Value);

        return Result<UserResponse>.Success(
            new UserResponse(user.Id, user.Email!, user.FirstName, user.LastName, roles, claimsDict));
    }
}

public static class RemoveClaimEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/users/{userId}/claims/{claimType}", async (string userId, string claimType, IMediator mediator) =>
        {
            var result = await mediator.Send(new RemoveClaimCommand(userId, claimType));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error, statusCode: result.StatusCode);
        }).RequireAuthorization(PolicyNames.AdminOnly);
    }
}
