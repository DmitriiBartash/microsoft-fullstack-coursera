using System.Security.Claims;
using EmployeeManagement.Api.Common.Authorization;
using EmployeeManagement.Api.Common.Models;
using EmployeeManagement.Api.Domain.Entities;
using EmployeeManagement.Api.Features.Employees.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Features.Employees.GetEmployees;

public record GetEmployeesQuery : IRequest<Result<List<EmployeeResponse>>>;

public class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, Result<List<EmployeeResponse>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetEmployeesHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<List<EmployeeResponse>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _httpContextAccessor.HttpContext?.User;
        if (currentUser is null) return Result<List<EmployeeResponse>>.Failure("Unauthorized", 401);

        var isAdmin = currentUser.IsInRole(AppRoles.Admin);
        var isManager = currentUser.IsInRole(AppRoles.Manager);

        if (!isAdmin && !isManager)
            return Result<List<EmployeeResponse>>.Failure("Forbidden", 403);

        var users = await _userManager.Users.ToListAsync(cancellationToken);
        var result = new List<EmployeeResponse>();

        foreach (var user in users)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var department = claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Department)?.Value ?? "N/A";

            if (isManager && !isAdmin)
            {
                var managerDept = currentUser.FindFirst(CustomClaimTypes.Department)?.Value;
                if (department != managerDept) continue;
            }

            result.Add(new EmployeeResponse(user.Id, user.Email!, user.FirstName, user.LastName, user.Position, department));
        }

        return Result<List<EmployeeResponse>>.Success(result);
    }
}

public static class GetEmployeesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/employees", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetEmployeesQuery());
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error, statusCode: result.StatusCode);
        }).RequireAuthorization(PolicyNames.ManagementAccess);
    }
}
