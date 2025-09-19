using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), 200)]
    public ActionResult<User> GetUser(int id)
    {
        return Ok(new User { Id = id, Name = $"User{id}" });
    }
}

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
