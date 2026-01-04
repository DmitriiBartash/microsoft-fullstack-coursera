using InventoryHub.Application.Common.Interfaces;
using InventoryHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=InventoryHub.db";

        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlite(connectionString, o =>
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<InventoryDbContext>());

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

        await context.Database.EnsureCreatedAsync();
        await DataSeeder.SeedAsync(context);
    }
}
