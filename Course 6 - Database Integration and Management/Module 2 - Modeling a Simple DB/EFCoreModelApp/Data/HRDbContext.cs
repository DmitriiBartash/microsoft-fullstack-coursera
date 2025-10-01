using Microsoft.EntityFrameworkCore;
using EFCoreModelApp.Models;

namespace EFCoreModelApp.Data;

public class HRDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=hr.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One-to-many relationship
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId);

        // Seed initial data
        modelBuilder.Entity<Department>().HasData(
            new Department { DepartmentId = 1, Name = "HR" },
            new Department { DepartmentId = 2, Name = "IT" }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee { EmployeeId = 1, Name = "Alice", Position = "HR Manager", DepartmentId = 1 },
            new Employee { EmployeeId = 2, Name = "Bob", Position = "Developer", DepartmentId = 2 }
        );
    }
}
