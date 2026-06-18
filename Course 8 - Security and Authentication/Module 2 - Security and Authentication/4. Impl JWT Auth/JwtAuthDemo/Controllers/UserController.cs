using JwtAuthCore;
using JwtAuthCore.Models;
using JwtAuthCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDemo.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly UserStore _users;
    private readonly TokenService _tokens;

    public UserController(UserStore users, TokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    [HttpPost("/login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _users.Find(request.Username, request.Password);
        if (user is null)
            return Unauthorized(new MessageResponse("Invalid username or password."));

        return Ok(new TokenResponse(_tokens.GenerateToken(user)));
    }

    [Authorize]
    [HttpGet("/secure-data")]
    public IActionResult SecureData()
    {
        var name = User.Identity?.Name ?? "user";
        return Ok(new MessageResponse($"You reached the secure endpoint, {name}. Your token checked out."));
    }
}
