using System.IdentityModel.Tokens.Jwt;
using EmployeeManagement.Api.Common.Authorization;
using EmployeeManagement.Api.Common.Models;
using EmployeeManagement.Api.Domain.Entities;
using EmployeeManagement.Api.Features.Employees.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Api.Features.Employees.GetProfile;

public record GetProfileQuery : IRequest<Result<EmployeeResponse>>;

public class GetProfileHandler : IRequestHandler<GetProfileQuery, Result<EmployeeResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetProfileHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<EmployeeResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (userId is null) return Result<EmployeeResponse>.Failure("Unauthorized", 401);

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Result<EmployeeResponse>.Failure("User not found", 404);

        var claims = await _userManager.GetClaimsAsync(user);
        var department = claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Department)?.Value ?? "N/A";

        return Result<EmployeeResponse>.Success(
            new EmployeeResponse(user.Id, user.Email!, user.FirstName, user.LastName, user.Position, department));
    }
}

public static class GetProfileEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/employees/me", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProfileQuery());
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error, statusCode: result.StatusCode);
        }).RequireAuthorization();
    }
}
