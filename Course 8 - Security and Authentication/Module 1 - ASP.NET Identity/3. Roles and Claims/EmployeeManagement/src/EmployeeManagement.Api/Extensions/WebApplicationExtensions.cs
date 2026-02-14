using EmployeeManagement.Api.Common.Middleware;
using EmployeeManagement.Api.Features.Auth.Login;
using EmployeeManagement.Api.Features.Auth.RefreshToken;
using EmployeeManagement.Api.Features.Auth.Register;
using EmployeeManagement.Api.Features.Employees.GetEmployees;
using EmployeeManagement.Api.Features.Employees.GetProfile;
using EmployeeManagement.Api.Features.Users.AddClaim;
using EmployeeManagement.Api.Features.Users.AssignRole;
using EmployeeManagement.Api.Features.Users.GetAllUsers;
using EmployeeManagement.Api.Features.Users.RemoveClaim;
using EmployeeManagement.Api.Features.Users.RemoveRole;

namespace EmployeeManagement.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseCustomMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        LoginEndpoint.Map(app);
        RegisterEndpoint.Map(app);
        RefreshTokenEndpoint.Map(app);

        GetAllUsersEndpoint.Map(app);
        AssignRoleEndpoint.Map(app);
        RemoveRoleEndpoint.Map(app);
        AddClaimEndpoint.Map(app);
        RemoveClaimEndpoint.Map(app);

        GetEmployeesEndpoint.Map(app);
        GetProfileEndpoint.Map(app);

        return app;
    }
}
