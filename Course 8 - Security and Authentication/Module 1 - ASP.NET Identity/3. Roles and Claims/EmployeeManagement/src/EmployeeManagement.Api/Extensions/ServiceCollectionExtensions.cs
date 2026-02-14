using System.Text;
using EmployeeManagement.Api.Common.Authorization;
using EmployeeManagement.Api.Common.Behaviors;
using EmployeeManagement.Api.Domain.Entities;
using EmployeeManagement.Api.Infrastructure.Data;
using EmployeeManagement.Api.Infrastructure.Seed;
using EmployeeManagement.Api.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace EmployeeManagement.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("EmployeeManagementDb"));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                RoleClaimType = System.Security.Claims.ClaimTypes.Role
            };
        });

        services.AddScoped<TokenService>();
        services.AddHostedService<SeedDataService>();
        services.AddHttpContextAccessor();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Employee Management API", Version = "v1" });

            const string schemeName = "Bearer";

            options.AddSecurityDefinition(schemeName, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT token"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(schemeName, document)] = new List<string>()
            });
        });

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.AdminOnly, policy =>
                policy.RequireRole(AppRoles.Admin));

            options.AddPolicy(PolicyNames.ManagementAccess, policy =>
                policy.RequireRole(AppRoles.Admin, AppRoles.Manager));

            options.AddPolicy(PolicyNames.RequireWriteAccess, policy =>
                policy.RequireClaim(CustomClaimTypes.AccessLevel, AccessLevels.Write, AccessLevels.Full));

            options.AddPolicy(PolicyNames.RequireFullAccess, policy =>
                policy.RequireClaim(CustomClaimTypes.AccessLevel, AccessLevels.Full));
        });

        return services;
    }
}
