using EmployeeManagement.Api.Common.Authorization;
using EmployeeManagement.Api.Common.Models;
using EmployeeManagement.Api.Domain.Entities;
using EmployeeManagement.Api.Features.Users.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Features.Users.GetAllUsers;

public record GetAllUsersQuery : IRequest<Result<List<UserResponse>>>;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserResponse>>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllUsersHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<List<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);
        var result = new List<UserResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var claimsDict = claims
                .Where(c => c.Type is CustomClaimTypes.Department or CustomClaimTypes.AccessLevel)
                .ToDictionary(c => c.Type, c => c.Value);

            result.Add(new UserResponse(user.Id, user.Email!, user.FirstName, user.LastName, roles, claimsDict));
        }

        return Result<List<UserResponse>>.Success(result);
    }
}

public static class GetAllUsersEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllUsersQuery());
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error, statusCode: result.StatusCode);
        }).RequireAuthorization(PolicyNames.AdminOnly);
    }
}
