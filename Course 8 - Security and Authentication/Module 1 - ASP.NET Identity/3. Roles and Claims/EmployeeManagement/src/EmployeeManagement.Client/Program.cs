using Blazored.LocalStorage;
using EmployeeManagement.Client;
using EmployeeManagement.Client.Auth;
using EmployeeManagement.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<JwtAuthorizationMessageHandler>();

var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? "https://localhost:7200";
builder.Services.AddHttpClient("Api", client =>
    client.BaseAddress = new Uri(apiBaseAddress))
    .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<UserManagementService>();

await builder.Build().RunAsync();
