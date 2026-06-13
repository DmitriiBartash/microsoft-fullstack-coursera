# Server-Side State Management in a Full-Stack Application

**Course 7 — Full-Stack Integration** · Module 3 · Lesson 2 · `You Try It!`

> Build a small **Blazor Server** app that keeps state on the server using two
> complementary techniques: **sessions** (per-user data that survives across requests)
> and **caching** (`IMemoryCache` for frequently accessed data). You'll wire up the
> middleware, a reusable `CacheService`, and components that read and write both stores.

---

## 🎯 Objective

By the end of this activity, you will be able to implement **session handling** and
**caching** techniques in a Blazor Server application to maintain server-side state
efficiently — configuring the middleware pipeline, registering services for dependency
injection, and consuming both stores from Razor components.

---

## 🗂️ What you will build

A Blazor Server project named **`BlazorServerApp`** that demonstrates both state stores:

| Piece                          | Responsibility                                                        |
| ------------------------------ | --------------------------------------------------------------------- |
| `Program.cs`                   | Configure distributed memory cache, sessions, and DI registrations    |
| `Services/CacheService.cs`     | Reusable wrapper over `IMemoryCache` to get/set frequently used data  |
| `Pages/FetchData.razor`        | Fetch weather data and cache it for 5 minutes via `CacheService`      |
| `Pages/Counter.razor`          | Persist a counter to **session storage** with `Blazored.SessionStorage` |

**Flow:** `Request  →  UseSession()  →  Component  →  CacheService (IMemoryCache)  →  cached data`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code with the C# Dev Kit extension
- NuGet package: `Blazored.SessionStorage`

---

## 🛠️ Steps

### Step 1 — Prepare the application

You'll create a small Blazor Server app in Visual Studio Code that demonstrates
server-side state management using sessions and caching.

```bash
dotnet new blazor -o BlazorServerApp
cd BlazorServerApp
code .
```

- Confirm the project builds, then **clear any placeholder content** you intend to
  replace in `Program.cs` before wiring up the new services.

### Step 2 — Configure session handling

Add middleware to enable sessions. Open `Program.cs` and register a **distributed memory
cache** (the backing store sessions require), configure the session **timeout to 30 minutes**
with secure cookie options, and add `app.UseSession()` to the pipeline.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Razor Components (Blazor Server)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Session handling needs a backing store
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);   // 30-minute timeout
    options.Cookie.HttpOnly = true;                    // not readable from JS
    options.Cookie.IsEssential = true;                 // required even without consent
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Add the session middleware to the pipeline
app.UseSession();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

> Order matters: `UseSession()` must come **after** routing/static files and **before** the
> endpoints that read session data.

### Step 3 — Create a caching service

Implement a reusable service for managing cached data. In the root of the project, create a
folder called **`Services`** and add **`CacheService.cs`**. The class uses `IMemoryCache` to
store and retrieve frequently accessed data behind a small `GetOrCreate` API.

```csharp
// Services/CacheService.cs
using Microsoft.Extensions.Caching.Memory;

namespace BlazorServerApp.Services;

// Summary: thin wrapper over IMemoryCache for typed get/set with an expiry.
public class CacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache) => _cache = cache;

    public T? Get<T>(string key) =>
        _cache.TryGetValue(key, out T? value) ? value : default;

    public void Set<T>(string key, T value, TimeSpan expiry) =>
        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        });

    public T GetOrCreate<T>(string key, TimeSpan expiry, Func<T> factory) =>
        _cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = expiry;
            return factory();
        })!;
}
```

### Step 4 — Register services

Make the session handling and caching service available to the application. Update
`Program.cs` so the **memory cache** and your **`CacheService`** live in the dependency
injection container and can be injected into components.

```csharp
using BlazorServerApp.Services;

// ...existing builder.Services registrations...

builder.Services.AddMemoryCache();                 // backs IMemoryCache
builder.Services.AddScoped<CacheService>();        // your reusable cache wrapper
```

- Register `CacheService` and verify it is ready to be injected into components.

### Step 5 — Create a component to use caching

Create a `Pages` folder and add **`FetchData.razor`**. Inject `CacheService`, fetch weather
data, and store it in the cache for **5 minutes** so repeat visits within that window reuse
the cached result instead of regenerating it.

```razor
@* Pages/FetchData.razor *@
@page "/fetchdata"
@using BlazorServerApp.Services
@inject CacheService Cache
@rendermode InteractiveServer

<h1>Weather forecast</h1>

@if (_forecasts is null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr><th>Date</th><th>Temp. (C)</th><th>Summary</th></tr>
        </thead>
        <tbody>
            @foreach (var f in _forecasts)
            {
                <tr>
                    <td>@f.Date.ToShortDateString()</td>
                    <td>@f.TemperatureC</td>
                    <td>@f.Summary</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private WeatherForecast[]? _forecasts;

    protected override void OnInitialized()
    {
        // Cache the forecast for 5 minutes; only regenerate when it expires.
        _forecasts = Cache.GetOrCreate(
            "weather-forecast",
            TimeSpan.FromMinutes(5),
            GenerateForecast);
    }

    private WeatherForecast[] GenerateForecast()
    {
        var summaries = new[] { "Freezing", "Cool", "Mild", "Warm", "Hot" };
        return Enumerable.Range(1, 5).Select(i => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(i),
            TemperatureC = Random.Shared.Next(-5, 35),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        }).ToArray();
    }

    private class WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string? Summary { get; set; }
    }
}
```

### Step 6 — Enable advanced session handling

Extend session management with persistent, user-friendly storage. Install the
**`Blazored.SessionStorage`** package, register it in DI, then update a component such as
**`Counter.razor`** to store and retrieve a value from session storage.

```bash
dotnet add package Blazored.SessionStorage
```

```csharp
// Program.cs
using Blazored.SessionStorage;

builder.Services.AddBlazoredSessionStorage();      // session storage service
```

```razor
@* Pages/Counter.razor *@
@page "/counter"
@using Blazored.SessionStorage
@inject ISessionStorageService SessionStorage
@rendermode InteractiveServer

<h1>Counter</h1>
<p>Current count: <strong>@_count</strong></p>
<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    private const string Key = "count";
    private int _count;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Restore the persisted count when the component first renders.
            _count = await SessionStorage.GetItemAsync<int>(Key);
            StateHasChanged();
        }
    }

    private async Task IncrementCount()
    {
        _count++;
        await SessionStorage.SetItemAsync(Key, _count);   // persist across refreshes
    }
}
```

### Step 7 — Test your application

Run and test the application to verify that session and caching functionality work
correctly.

```bash
dotnet run
```

- Open the application in a browser. Visit `/fetchdata` and refresh within 5 minutes — the
  rows stay identical because the result is served from the cache.
- Visit `/counter`, click a few times, then refresh the page — the count is restored from
  session storage instead of resetting to zero.

---

## ▶️ Expected result

The app runs and both state stores behave as designed: cached weather data is reused for
5 minutes, and the counter value survives page refreshes via session storage — confirming
server-side state is maintained efficiently across requests.

---

## ☑️ Definition of done

- [ ] `BlazorServerApp` created with `dotnet new blazor` and opened in VS Code
- [ ] `Program.cs` registers `AddDistributedMemoryCache`, `AddSession` (30-min timeout, secure cookie) and calls `app.UseSession()`
- [ ] `Services/CacheService.cs` wraps `IMemoryCache` and is registered with `AddMemoryCache` + `AddScoped<CacheService>()`
- [ ] `Pages/FetchData.razor` injects `CacheService` and caches weather data for 5 minutes
- [ ] `Blazored.SessionStorage` is installed, registered, and used by `Pages/Counter.razor`
- [ ] `dotnet run` works; caching and session persistence verified in the browser

---

## 🔑 Key concepts

- **Server-side state lives on the server** — Blazor Server keeps component state in a
  server circuit; sessions and caching extend that to survive across requests and
  reconnections rather than relying on the client.
- **Sessions need a backing store** — `AddSession` does nothing on its own; it requires a
  distributed cache (`AddDistributedMemoryCache`) and the `UseSession()` middleware placed
  correctly in the pipeline.
- **Caching is about reuse, not persistence** — `IMemoryCache` with an absolute expiry
  (5 minutes here) avoids regenerating expensive data, but entries can be evicted at any
  time, so always provide a factory to rebuild them.
- **Secure cookies by default** — `HttpOnly`, `IsEssential`, and `SecurePolicy.Always`
  protect the session cookie from script access and force it over HTTPS.
- **Pick the right tool per concern** — caching for shared, regeneratable data;
  session storage for per-user values that should follow the user across navigation.
