using backend.Endpoints;
using backend.Services;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5115",  
                "https://localhost:7246"  
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<ProductService>();
builder.Services.AddOpenApi();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.WriteIndented = true;
});

builder.WebHost.UseUrls("http://localhost:5100", "https://localhost:7100");

var app = builder.Build();


app.UseCors("AllowFrontend");

app.MapProductEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Product API";
        options.Theme = ScalarTheme.Moon;
    });
}

app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();

app.Run();
