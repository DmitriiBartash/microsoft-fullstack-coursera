using System.Security.Claims;
using EmployeeManagement.Api.Common.Authorization;
using EmployeeManagement.Api.Common.Models;
using EmployeeManagement.Api.Domain.Entities;
using EmployeeManagement.Api.Features.Users.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Api.Features.Users.AddClaim;

public record AddClaimCommand(string UserId, string ClaimType, string ClaimValue) : IRequest<Result<UserResponse>>;

public class AddClaimValidator : AbstractValidator<AddClaimCommand>
{
    public AddClaimValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ClaimType).NotEmpty()
            .Must(t => t is CustomClaimTypes.Department or CustomClaimTypes.AccessLevel)
            .WithMessage("ClaimType must be Department or AccessLevel");
        RuleFor(x => x.ClaimValue).NotEmpty();
    }
}

public class AddClaimHandler : IRequestHandler<AddClaimCommand, Result<UserResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AddClaimHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserResponse>> Handle(AddClaimCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) return Result<UserResponse>.Failure("User not found", 404);

        var existingClaims = await _userManager.GetClaimsAsync(user);
        var existingClaim = existingClaims.FirstOrDefault(c => c.Type == request.ClaimType);
        if (existingClaim is not null)
            await _userManager.RemoveClaimAsync(user, existingClaim);

        await _userManager.AddClaimAsync(user, new Claim(request.ClaimType, request.ClaimValue));

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        var claimsDict = claims
            .Where(c => c.Type is CustomClaimTypes.Department or CustomClaimTypes.AccessLevel)
            .ToDictionary(c => c.Type, c => c.Value);

        return Result<UserResponse>.Success(
            new UserResponse(user.Id, user.Email!, user.FirstName, user.LastName, roles, claimsDict));
    }
}

public static class AddClaimEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/{userId}/claims", async (string userId, AddClaimRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new AddClaimCommand(userId, request.ClaimType, request.ClaimValue));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error, statusCode: result.StatusCode);
        }).RequireAuthorization(PolicyNames.AdminOnly);
    }
}

public record AddClaimRequest(string ClaimType, string ClaimValue);
