using JwtSecurityCore;
using JwtSecurityCore.Models;
using JwtSecurityCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthExample.Controllers;

/// <summary>
/// Issues short-lived access tokens and manages the refresh-token lifecycle. The refresh token is
/// never returned in the body — it is set as an HttpOnly, Secure, SameSite=Strict cookie scoped to
/// <see cref="RefreshCookieDefaults.Path"/>, so script running in the page cannot read it and it is
/// only ever sent to these endpoints.
/// </summary>
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserStore _users;
    private readonly TokenService _tokens;
    private readonly RefreshTokenStore _refresh;
    private readonly JwtSettings _settings;

    public AuthController(UserStore users, TokenService tokens, RefreshTokenStore refresh, JwtSettings settings)
    {
        _users = users;
        _tokens = tokens;
        _refresh = refresh;
        _settings = settings;
    }

    /// <summary>Validates credentials, returns a signed access token, and sets the refresh-token cookie.</summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _users.Find(request.Username, request.Password);
        if (user is null)
            return Unauthorized(new MessageResponse("Invalid username or password."));

        var refresh = _refresh.Issue(user.Username);
        SetRefreshCookie(refresh.Token, refresh.ExpiresUtc);
        return Ok(new TokenResponse(_tokens.GenerateToken(user), _settings.AccessTokenMinutes * 60));
    }

    /// <summary>
    /// Exchanges a valid refresh-token cookie for a fresh access token, rotating the refresh token.
    /// Returns 401 when the cookie is missing, expired, already used, or its user no longer exists.
    /// </summary>
    [HttpPost("refresh")]
    public IActionResult Refresh()
    {
        var rotated = _refresh.Rotate(Request.Cookies[RefreshCookieDefaults.Name]);
        var user = rotated is null ? null : _users.All.FirstOrDefault(u => u.Username == rotated.Username);
        if (rotated is null || user is null)
        {
            ClearRefreshCookie();
            return Unauthorized(new MessageResponse("Refresh token missing, expired, or already used."));
        }

        SetRefreshCookie(rotated.Token, rotated.ExpiresUtc);
        return Ok(new TokenResponse(_tokens.GenerateToken(user), _settings.AccessTokenMinutes * 60));
    }

    /// <summary>Revokes the current refresh token and clears the cookie.</summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _refresh.Revoke(Request.Cookies[RefreshCookieDefaults.Name]);
        ClearRefreshCookie();
        return Ok(new MessageResponse("Logged out — refresh token revoked."));
    }

    private void SetRefreshCookie(string token, DateTime expiresUtc) =>
        Response.Cookies.Append(RefreshCookieDefaults.Name, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = RefreshCookieDefaults.Path,
            Expires = expiresUtc
        });

    private void ClearRefreshCookie() =>
        Response.Cookies.Delete(RefreshCookieDefaults.Name, new CookieOptions { Path = RefreshCookieDefaults.Path });
}
