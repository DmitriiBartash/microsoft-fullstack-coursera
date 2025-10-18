using frontend;
using frontend.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

const string backendUrl = "https://localhost:7100/";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(backendUrl)
});

builder.Services.AddScoped<ProductService>();

await builder.Build().RunAsync();
