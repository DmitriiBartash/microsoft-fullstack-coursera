using Microsoft.AspNetCore.Mvc;
using UserManagementApp.Models;

namespace UserManagementApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(ILogger<UsersController> logger) : ControllerBase
    {
        private static readonly List<User> Users = new();
        private static int _nextId = 0;
        private static readonly object _sync = new();

        private readonly ILogger<UsersController> _logger = logger;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            List<User> snapshot;
            lock (_sync)
            {
                snapshot = Users.ToList();
            }
            return Ok(snapshot);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<User> GetUser(int id)
        {
            User? user;
            lock (_sync)
            {
                user = Users.FirstOrDefault(u => u.Id == id);
            }
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<User> CreateUser([FromBody] User user)
        {
            if (user is null) return BadRequest();

            user.FirstName = user.FirstName?.Trim() ?? string.Empty;
            user.LastName = user.LastName?.Trim() ?? string.Empty;
            user.Email = user.Email?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(user.FirstName))
                ModelState.AddModelError(nameof(user.FirstName), "First name is required");
            if (string.IsNullOrWhiteSpace(user.LastName))
                ModelState.AddModelError(nameof(user.LastName), "Last name is required");
            if (string.IsNullOrWhiteSpace(user.Email))
                ModelState.AddModelError(nameof(user.Email), "Email is required");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            lock (_sync)
            {
                if (Users.Any(u => string.Equals(u.Email, user.Email, StringComparison.OrdinalIgnoreCase)))
                    return Conflict(new { message = "Email must be unique." });

                user.Id = Interlocked.Increment(ref _nextId);
                Users.Add(user);
            }

            _logger.LogInformation("Created user {UserId}", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (updatedUser is null) return BadRequest();

            updatedUser.FirstName = updatedUser.FirstName?.Trim() ?? string.Empty;
            updatedUser.LastName = updatedUser.LastName?.Trim() ?? string.Empty;
            updatedUser.Email = updatedUser.Email?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(updatedUser.FirstName))
                ModelState.AddModelError(nameof(updatedUser.FirstName), "First name is required");
            if (string.IsNullOrWhiteSpace(updatedUser.LastName))
                ModelState.AddModelError(nameof(updatedUser.LastName), "Last name is required");
            if (string.IsNullOrWhiteSpace(updatedUser.Email))
                ModelState.AddModelError(nameof(updatedUser.Email), "Email is required");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            lock (_sync)
            {
                var user = Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();

                if (Users.Any(u => u.Id != id &&
                                   string.Equals(u.Email, updatedUser.Email, StringComparison.OrdinalIgnoreCase)))
                    return Conflict(new { message = "Email must be unique." });

                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Email = updatedUser.Email;
            }

            _logger.LogInformation("Updated user {UserId}", id);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteUser(int id)
        {
            bool removed = false;
            lock (_sync)
            {
                var user = Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();
                removed = Users.Remove(user);
            }

            if (removed)
                _logger.LogInformation("Deleted user {UserId}", id);

            return NoContent();
        }
    }
}
