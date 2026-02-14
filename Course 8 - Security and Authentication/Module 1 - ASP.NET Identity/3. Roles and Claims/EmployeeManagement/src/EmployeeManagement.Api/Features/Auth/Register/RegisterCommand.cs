using System.Security.Claims;
using EmployeeManagement.Api.Common.Authorization;
using EmployeeManagement.Api.Common.Models;
using EmployeeManagement.Api.Domain.Entities;
using EmployeeManagement.Api.Features.Auth.Contracts;
using EmployeeManagement.Api.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Api.Features.Auth.Register;

public record RegisterCommand(string Email, string Password, string FirstName, string LastName) : IRequest<Result<AuthResponse>>;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
    }
}

public class RegisterHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;

    public RegisterHandler(UserManager<ApplicationUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
            return Result<AuthResponse>.Failure("User with this email already exists");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Position = "Employee",
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return Result<AuthResponse>.Failure(string.Join("; ", createResult.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, AppRoles.Employee);
        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Department, Departments.IT));
        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.AccessLevel, AccessLevels.Read));

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(user, roles, claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(user);

        return Result<AuthResponse>.Success(
            new AuthResponse(accessToken, refreshToken.Token, DateTime.UtcNow.AddMinutes(30)));
    }
}

public static class RegisterEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (RegisterCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error, statusCode: result.StatusCode);
        }).AllowAnonymous();
    }
}
