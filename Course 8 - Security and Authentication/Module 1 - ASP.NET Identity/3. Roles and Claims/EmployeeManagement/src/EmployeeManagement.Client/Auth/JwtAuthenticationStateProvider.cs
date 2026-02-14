using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace EmployeeManagement.Client.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public JwtAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsStringAsync("accessToken");

        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(_anonymous);

        token = token.Trim('"');

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
            return new AuthenticationState(_anonymous);

        var jwt = handler.ReadJwtToken(token);
        if (jwt.ValidTo < DateTime.UtcNow)
        {
            await _localStorage.RemoveItemAsync("accessToken");
            await _localStorage.RemoveItemAsync("refreshToken");
            return new AuthenticationState(_anonymous);
        }

        var claims = jwt.Claims.ToList();
        var roleClaims = claims
            .Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
            .Select(c => new Claim(ClaimTypes.Role, c.Value));

        var identity = new ClaimsIdentity(claims, "jwt");
        foreach (var role in roleClaims)
        {
            if (!identity.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role.Value))
                identity.AddClaim(role);
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void MarkUserAsAuthenticated(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var claims = jwt.Claims.ToList();

        var identity = new ClaimsIdentity(claims, "jwt");
        var roleClaims = claims
            .Where(c => c.Type == "role")
            .Select(c => new Claim(ClaimTypes.Role, c.Value));
        foreach (var role in roleClaims)
            identity.AddClaim(role);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public void MarkUserAsLoggedOut()
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
