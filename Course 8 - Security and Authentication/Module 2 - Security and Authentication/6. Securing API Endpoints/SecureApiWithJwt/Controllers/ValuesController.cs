using SecureApiCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureApiWithJwt.Controllers;

[ApiController]
[Route("values")]
public class ValuesController : ControllerBase
{
    /// <summary>Open to everyone — no token required. The public baseline of the access matrix.</summary>
    [AllowAnonymous]
    [HttpGet("public")]
    public IActionResult Public() =>
        Ok(new MessageResponse("Public data — anyone can read this, no token required."));

    /// <summary>Any authenticated caller in the Admin or User role.</summary>
    [Authorize(Roles = "Admin,User")]
    [HttpGet("user")]
    public IActionResult Members() =>
        Ok(new MessageResponse($"Member data for {User.Identity?.Name}. Requires the User or Admin role."));

    /// <summary>Admin role only — a User-role token is rejected with 403 Forbidden.</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminArea() =>
        Ok(new MessageResponse($"Admin console for {User.Identity?.Name}. Requires the Admin role."));
}
