using Microsoft.AspNetCore.Identity;

namespace SecureNotes.Api.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public List<Note> Notes { get; set; } = [];
}
