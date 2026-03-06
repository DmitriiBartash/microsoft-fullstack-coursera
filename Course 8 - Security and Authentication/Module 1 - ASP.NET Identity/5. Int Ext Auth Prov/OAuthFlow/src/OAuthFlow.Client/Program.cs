using OAuthFlow.Client.Models;
using OAuthFlow.Client.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OAuthSettings>(builder.Configuration.GetSection("OAuth"));
builder.Services.AddHttpClient<IOAuthService, OAuthService>();
builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapRazorPages();

app.Run();
