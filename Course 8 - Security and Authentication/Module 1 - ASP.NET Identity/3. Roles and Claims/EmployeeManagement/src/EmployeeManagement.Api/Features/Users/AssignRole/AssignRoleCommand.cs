using EmployeeManagement.Api.Common.Authorization;
using EmployeeManagement.Api.Common.Models;
using EmployeeManagement.Api.Domain.Entities;
using EmployeeManagement.Api.Features.Users.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Api.Features.Users.AssignRole;

public record AssignRoleCommand(string UserId, string Role) : IRequest<Result<UserResponse>>;

public class AssignRoleValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty().Must(r => r is AppRoles.Admin or AppRoles.Manager or AppRoles.Employee)
            .WithMessage("Role must be Admin, Manager, or Employee");
    }
}

public class AssignRoleHandler : IRequestHandler<AssignRoleCommand, Result<UserResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AssignRoleHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserResponse>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) return Result<UserResponse>.Failure("User not found", 404);

        if (await _userManager.IsInRoleAsync(user, request.Role))
            return Result<UserResponse>.Failure($"User already has role '{request.Role}'");

        await _userManager.AddToRoleAsync(user, request.Role);

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        var claimsDict = claims
            .Where(c => c.Type is CustomClaimTypes.Department or CustomClaimTypes.AccessLevel)
            .ToDictionary(c => c.Type, c => c.Value);

        return Result<UserResponse>.Success(
            new UserResponse(user.Id, user.Email!, user.FirstName, user.LastName, roles, claimsDict));
    }
}

public static class AssignRoleEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/{userId}/roles", async (string userId, AssignRoleRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new AssignRoleCommand(userId, request.Role));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error, statusCode: result.StatusCode);
        }).RequireAuthorization(PolicyNames.AdminOnly);
    }
}

public record AssignRoleRequest(string Role);
