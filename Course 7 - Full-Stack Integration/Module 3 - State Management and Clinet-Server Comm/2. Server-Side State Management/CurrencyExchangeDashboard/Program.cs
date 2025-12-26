using Blazored.SessionStorage;
using CurrencyExchangeDashboard.Components;
using CurrencyExchangeDashboard.Services;
using CurrencyExchangeDashboard.Services.Interfaces;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add Blazored SessionStorage
builder.Services.AddBlazoredSessionStorage();

// Add Memory Cache (for server-side caching)
builder.Services.AddMemoryCache();

// Add HTTP client for API calls
builder.Services.AddHttpClient<IExchangeRateService, ExchangeRateService>();

// Register application services
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddScoped<IUserStateService, UserStateService>();

// Configure session handling (as per lab requirements)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Use session middleware (as per lab requirements)
app.UseSession();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
