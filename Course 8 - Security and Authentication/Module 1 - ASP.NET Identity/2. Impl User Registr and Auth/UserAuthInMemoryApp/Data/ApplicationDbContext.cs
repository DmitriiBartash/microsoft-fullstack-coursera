using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UserAuthInMemoryApp.Data;

/// <summary>
/// Database context for ASP.NET Identity with In-Memory storage.
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
