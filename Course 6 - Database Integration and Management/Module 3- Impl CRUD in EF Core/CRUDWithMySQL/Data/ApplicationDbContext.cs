using Microsoft.EntityFrameworkCore;
using CRUDWithMySQL.Models;

namespace CRUDWithMySQL.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString =
            "Server=localhost;Database=ProductDB;User=efuser;Password=efpassword;";

        optionsBuilder.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 36))
        );
    }
}
