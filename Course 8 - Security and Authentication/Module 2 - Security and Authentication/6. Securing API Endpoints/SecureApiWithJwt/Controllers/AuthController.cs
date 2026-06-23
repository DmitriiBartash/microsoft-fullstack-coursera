using SecureApiCore;
using SecureApiCore.Models;
using SecureApiCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace SecureApiWithJwt.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserStore _users;
    private readonly TokenService _tokens;

    public AuthController(UserStore users, TokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    /// <summary>Validates credentials and returns a signed JWT carrying the user's roles.</summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _users.Find(request.Username, request.Password);
        if (user is null)
            return Unauthorized(new MessageResponse("Invalid username or password."));

        return Ok(new TokenResponse(_tokens.GenerateToken(user)));
    }
}
