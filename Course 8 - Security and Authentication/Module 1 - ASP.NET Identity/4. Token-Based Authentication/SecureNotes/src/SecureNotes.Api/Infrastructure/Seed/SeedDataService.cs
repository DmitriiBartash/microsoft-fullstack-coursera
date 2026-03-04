using Microsoft.AspNetCore.Identity;
using SecureNotes.Api.Domain.Entities;
using SecureNotes.Api.Infrastructure.Data;

namespace SecureNotes.Api.Infrastructure.Seed;

public class SeedDataService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext)
{
    public async Task SeedAsync()
    {
        if (dbContext.Users.Any()) return;

        var demo = await CreateUserAsync("demo@notes.com", "Demo123", "Demo User");
        var alice = await CreateUserAsync("alice@notes.com", "Alice123", "Alice");

        dbContext.Notes.AddRange(
            new Note { Title = "Welcome to SecureNotes", Content = "This app demonstrates JWT token-based authentication with access and refresh tokens.", IsPinned = true, UserId = demo.Id },
            new Note { Title = "Token Lifecycle", Content = "Access tokens expire in 5 minutes. Watch the countdown in the JWT Inspector! When it expires, a refresh token is used to get a new access token.", UserId = demo.Id },
            new Note { Title = "Security Best Practices", Content = "Never store sensitive data in JWT payload — it's base64-encoded, not encrypted. Use HTTPS in production. Rotate refresh tokens on each use.", UserId = demo.Id },
            new Note { Title = "Alice's Private Note", Content = "This note belongs to Alice and should not be visible to other users. Owner isolation is enforced at the API level.", IsPinned = true, UserId = alice.Id },
            new Note { Title = "Cross-user Protection", Content = "Try accessing another user's notes — the API will return 404, not 403, to avoid leaking information about note existence.", UserId = alice.Id }
        );

        await dbContext.SaveChangesAsync();
    }

    private async Task<ApplicationUser> CreateUserAsync(string email, string password, string displayName)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName
        };

        await userManager.CreateAsync(user, password);
        return user;
    }
}
