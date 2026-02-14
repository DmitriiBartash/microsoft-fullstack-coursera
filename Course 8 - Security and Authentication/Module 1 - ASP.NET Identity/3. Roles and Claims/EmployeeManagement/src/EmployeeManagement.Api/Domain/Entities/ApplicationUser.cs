using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Api.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}
