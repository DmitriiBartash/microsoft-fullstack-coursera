using SecureDataCore.Models;
using SecureDataStudio.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Per-circuit secure storage, seeded with the AES key from configuration.
builder.Services.AddScoped(sp =>
{
    var keyBase64 = sp.GetRequiredService<IConfiguration>()["Encryption:Key"]
        ?? throw new InvalidOperationException("Encryption:Key is not configured.");
    return new SecureStorage(Convert.FromBase64String(keyBase64));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
