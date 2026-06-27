using JwtSecurityCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthExample.Controllers;

/// <summary>
/// Protected resources. <c>[Authorize]</c> requires a valid (unexpired, untampered) access token —
/// a missing or expired token yields 401, which is the client's cue to refresh. The admin route adds
/// a role requirement, so a valid User-role token is rejected with 403.
/// </summary>
[ApiController]
[Route("secure")]
public class SecureController : ControllerBase
{
    /// <summary>Any authenticated caller.</summary>
    [Authorize]
    [HttpGet]
    public IActionResult Protected() =>
        Ok(new MessageResponse($"Protected data for {User.Identity?.Name}. A valid access token unlocked this."));

    /// <summary>Admin role only — a valid User-role token is rejected with 403 Forbidden.</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminArea() =>
        Ok(new MessageResponse($"Admin area for {User.Identity?.Name}. Requires the Admin role."));
}
