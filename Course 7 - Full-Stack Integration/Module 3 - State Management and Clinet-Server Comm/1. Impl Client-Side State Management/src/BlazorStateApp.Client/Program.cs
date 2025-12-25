using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using TG.Blazor.IndexedDB;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using BlazorStateApp.Client;
using BlazorStateApp.Client.Services.Storage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseRouting();

#if DEBUG
    options.UseReduxDevTools(rdt =>
    {
        rdt.Name = "BlazorStateApp";
        rdt.EnableStackTrace();
    });
#endif
});

builder.Services.AddIndexedDB(dbStore =>
{
    dbStore.DbName = "BlazorStateAppDb";
    dbStore.Version = 1;

    dbStore.Stores.Add(new StoreSchema
    {
        Name = "Settings",
        PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = false }
    });

    dbStore.Stores.Add(new StoreSchema
    {
        Name = "CartItems",
        PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = true }
    });
});

builder.Services.AddScoped<IStorageService, IndexedDbStorageService>();

await builder.Build().RunAsync();
