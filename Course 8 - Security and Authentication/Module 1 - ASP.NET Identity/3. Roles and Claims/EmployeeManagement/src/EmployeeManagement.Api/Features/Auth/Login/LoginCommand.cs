using EmployeeManagement.Api.Common.Models;
using EmployeeManagement.Api.Domain.Entities;
using EmployeeManagement.Api.Features.Auth.Contracts;
using EmployeeManagement.Api.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;

    public LoginHandler(UserManager<ApplicationUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Result<AuthResponse>.Failure("Invalid email or password", 401);

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

public static class LoginEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (LoginCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error, statusCode: result.StatusCode);
        }).AllowAnonymous();
    }
}
