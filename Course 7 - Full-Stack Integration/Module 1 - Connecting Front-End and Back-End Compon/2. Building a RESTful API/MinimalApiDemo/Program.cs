using Scalar.AspNetCore;
using MinimalApiDemo.Endpoints;
using MinimalApiDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Register OpenAPI & Scalar
builder.Services.AddOpenApi();
builder.Services.AddSingleton<TaskService>();

var app = builder.Build();

// Map OpenAPI + Scalar UI
app.MapOpenApi();
app.MapScalarApiReference();

// Redirect root ("/") to /scalar
app.MapGet("/", () => Results.Redirect("/scalar"));

// Register all endpoints
app.MapTaskEndpoints();

app.Run();
