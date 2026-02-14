using System.Security.Claims;
using EmployeeManagement.Api.Common.Authorization;
using EmployeeManagement.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Api.Infrastructure.Seed;

public class SeedDataService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public SeedDataService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedRoles(roleManager);
        await SeedUsers(userManager);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = [AppRoles.Admin, AppRoles.Manager, AppRoles.Employee];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedUsers(UserManager<ApplicationUser> userManager)
    {
        var users = new[]
        {
            new { Email = "admin@hr.com", FirstName = "Alex", LastName = "Admin", Position = "System Administrator", Role = AppRoles.Admin, Department = Departments.IT, AccessLevel = AccessLevels.Full },
            new { Email = "manager.it@hr.com", FirstName = "Ivan", LastName = "Petrov", Position = "IT Manager", Role = AppRoles.Manager, Department = Departments.IT, AccessLevel = AccessLevels.Write },
            new { Email = "manager.hr@hr.com", FirstName = "Maria", LastName = "Sidorova", Position = "HR Manager", Role = AppRoles.Manager, Department = Departments.HR, AccessLevel = AccessLevels.Write },
            new { Email = "manager.finance@hr.com", FirstName = "Dmitry", LastName = "Volkov", Position = "Finance Manager", Role = AppRoles.Manager, Department = Departments.Finance, AccessLevel = AccessLevels.Write },
            new { Email = "dev@hr.com", FirstName = "Sergey", LastName = "Kozlov", Position = "Software Developer", Role = AppRoles.Employee, Department = Departments.IT, AccessLevel = AccessLevels.Read },
            new { Email = "recruiter@hr.com", FirstName = "Anna", LastName = "Morozova", Position = "Recruiter", Role = AppRoles.Employee, Department = Departments.HR, AccessLevel = AccessLevels.Read },
            new { Email = "analyst@hr.com", FirstName = "Elena", LastName = "Sokolova", Position = "Financial Analyst", Role = AppRoles.Employee, Department = Departments.Finance, AccessLevel = AccessLevels.Read }
        };

        foreach (var u in users)
        {
            if (await userManager.FindByEmailAsync(u.Email) is not null) continue;

            var user = new ApplicationUser
            {
                UserName = u.Email,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Position = u.Position,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Demo123!");
            if (!result.Succeeded) continue;

            await userManager.AddToRoleAsync(user, u.Role);
            await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Department, u.Department));
            await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.AccessLevel, u.AccessLevel));
        }
    }
}
