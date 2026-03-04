using SecureNotes.Api.Extensions;
using SecureNotes.Api.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddScoped<SeedDataService>();

var app = builder.Build();

app.ConfigurePipeline();
await app.SeedDataAsync();

app.Run();
