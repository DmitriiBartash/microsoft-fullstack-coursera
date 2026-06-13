# Handling API Responses and State Management in Blazor

**Course 7 — Full-Stack Integration** · Module 2 · Lesson 2 · `You Try It!`

> Extend your Blazor WebAssembly **`WeatherApp`** so it consumes a *second* API
> (JSONPlaceholder users) alongside the weather feed, then make the two stay in
> sync through a shared **`WeatherStateService`**. You'll deserialize JSON with
> `HttpClient`, cancel stale requests with a `CancellationToken`, broadcast
> changes with an `Action` delegate, and re-render with `StateHasChanged()`.

---

## 🎯 Objective

Learn how to handle live API responses and manage **shared state** in a Blazor
WebAssembly app: fetch and deserialize JSON from multiple endpoints, store the
results in a singleton state service, notify components when that state changes,
and drive the UI to update dynamically — including a loading indicator and
cancellation of overlapping requests.

---

## 🗂️ What you will build

You extend the existing **`WeatherApp`** project with one new page, two models,
and a shared state service:

| File                            | Folder      | Responsibility                                                        |
| ------------------------------- | ----------- | --------------------------------------------------------------------- |
| `DynamicWeather.razor`          | `Pages/`    | Page that fetches both APIs, shows a loader, and renders live data    |
| `Models/User.cs`                | `Models/`   | Model for the JSONPlaceholder `users` response                        |
| `Models/WeatherData.cs`         | `Models/`   | Model for the weather API response                                    |
| `Services/WeatherStateService.cs` | `Services/` | Hold weather + user data and **notify** subscribers on change         |

**Flow:** `Button → fetch (HttpClient + CancellationToken) → deserialize → WeatherStateService → OnChange event → StateHasChanged() → UI`

---

## ✅ Prerequisites

- The **`WeatherApp`** Blazor WebAssembly project from the previous activity, with a working `WeatherFetch.razor` component already fetching from the weather API.
- .NET SDK installed — check with `dotnet --version`.
- Visual Studio Code (or Visual Studio).
- A free, no-auth public API for the second feed: `https://jsonplaceholder.typicode.com/users`.

> The `System.Net.Http.Json` extension methods (`GetFromJsonAsync`) ship with the Blazor WebAssembly SDK, so no extra NuGet package is required.

---

## 🛠️ Steps

### Step 1 — Extend the Blazor application

Open the **`WeatherApp`** project from the previous activity and confirm the
existing `WeatherFetch.razor` component still fetches data correctly. Then add a
new page for the dynamic, state-driven view.

- Create a new Razor component named **`DynamicWeather.razor`** in the `Pages` folder.
- Create a **`Models`** folder and a **`Services`** folder at the project root if they do not already exist.

```bash
cd WeatherApp
mkdir Models Services
```

Give the new page a route and a placeholder so it compiles before the data wiring is in place:

```razor
@page "/dynamic-weather"

<PageTitle>Dynamic Weather</PageTitle>

<h1>Dynamic Weather &amp; Users</h1>

<p>Coming soon…</p>
```

### Step 2 — Fetch and deserialize a JSON response

You'll pull placeholder **user** data from JSONPlaceholder and deserialize it
into a strongly-typed model.

- Add a **`User`** class to the `Models` folder that mirrors the JSONPlaceholder response.

```csharp
// Models/User.cs
using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
```

- **Inject `HttpClient`** into `DynamicWeather.razor` and add a method that fetches the users in `OnInitializedAsync`. Pass a `CancellationToken` so a new request can cancel any previous in-flight call.

```razor
@page "/dynamic-weather"
@using WeatherApp.Models
@using System.Net.Http.Json
@inject HttpClient Http
@implements IDisposable

<PageTitle>Dynamic Weather</PageTitle>

<h1>Dynamic Weather &amp; Users</h1>

@code {
    private List<User>? users;
    private CancellationTokenSource? cts;

    protected override async Task OnInitializedAsync()
    {
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        // Cancel any request that is still running before starting a new one.
        cts?.Cancel();
        cts = new CancellationTokenSource();

        try
        {
            users = await Http.GetFromJsonAsync<List<User>>(
                "https://jsonplaceholder.typicode.com/users", cts.Token);
        }
        catch (OperationCanceledException)
        {
            // A newer request superseded this one — ignore.
        }
    }

    public void Dispose() => cts?.Cancel();
}
```

### Step 3 — Implement state management for API data updates

Create a service that holds the weather and user data, and raises an event when
either changes so any subscribed component can re-render.

- Add a **`WeatherData`** class to the `Models` folder to model the weather API response.

```csharp
// Models/WeatherData.cs
using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class WeatherData
{
    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("temperatureC")]
    public double TemperatureC { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;
}
```

- Add a **`WeatherStateService`** class to the `Services` folder. It stores both datasets and uses an `Action` delegate (`OnChange`) to notify components of state changes.

```csharp
// Services/WeatherStateService.cs
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherStateService
{
    public WeatherData? Weather { get; private set; }
    public List<User> Users { get; private set; } = new();

    // Components subscribe to this to know when to re-render.
    public event Action? OnChange;

    public void SetWeather(WeatherData weather)
    {
        Weather = weather;
        NotifyStateChanged();
    }

    public void SetUsers(List<User> users)
    {
        Users = users;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
```

- Register the state service as a singleton in **`Program.cs`** so every component shares one instance:

```csharp
// Program.cs
using WeatherApp.Services;

builder.Services.AddSingleton<WeatherStateService>();
```

- **Inject the service** into `DynamicWeather.razor` and subscribe to its `OnChange` event, calling `StateHasChanged` whenever the shared state updates. Always unsubscribe in `Dispose` to avoid leaks.

```razor
@inject WeatherStateService State

@code {
    protected override void OnInitialized()
    {
        // Re-render this component whenever the shared state changes.
        State.OnChange += StateHasChanged;
    }

    // Add to the existing Dispose():
    // State.OnChange -= StateHasChanged;
}
```

### Step 4 — Test UI updates dynamically based on new API responses

Wire up triggers, push freshly fetched data into the service, and render it with
a loading indicator.

- Add **buttons** that fetch new data from both APIs on demand.
- After each fetch, push the result into `WeatherStateService` — its `OnChange` event invokes `StateHasChanged()` automatically. Use `StateHasChanged()` directly inside the loading branch so the spinner appears immediately.
- Show a **loading indicator** while an API call is in progress, then display the weather and user data.

```razor
@page "/dynamic-weather"
@using WeatherApp.Models
@using WeatherApp.Services
@using System.Net.Http.Json
@inject HttpClient Http
@inject WeatherStateService State
@implements IDisposable

<PageTitle>Dynamic Weather</PageTitle>

<h1>Dynamic Weather &amp; Users</h1>

<button class="btn btn-primary" @onclick="RefreshUsersAsync" disabled="@isLoading">
    Refresh Users
</button>
<button class="btn btn-secondary" @onclick="RefreshWeatherAsync" disabled="@isLoading">
    Refresh Weather
</button>

@if (isLoading)
{
    <p><em>Loading…</em></p>
}
else
{
    @if (State.Weather is not null)
    {
        <h2>Weather</h2>
        <p>@State.Weather.Location: @State.Weather.TemperatureC °C — @State.Weather.Summary</p>
    }

    <h2>Users (@State.Users.Count)</h2>
    <ul>
        @foreach (var user in State.Users)
        {
            <li>@user.Name (@user.Email)</li>
        }
    </ul>
}

@code {
    private bool isLoading;
    private CancellationTokenSource? cts;

    protected override void OnInitialized()
    {
        State.OnChange += StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        await RefreshUsersAsync();
    }

    private async Task RefreshUsersAsync()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();

        isLoading = true;
        StateHasChanged();   // show the loader right away

        try
        {
            var result = await Http.GetFromJsonAsync<List<User>>(
                "https://jsonplaceholder.typicode.com/users", cts.Token);

            if (result is not null)
            {
                State.SetUsers(result);   // raises OnChange → StateHasChanged
            }
        }
        catch (OperationCanceledException)
        {
            // Superseded by a newer request — ignore.
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task RefreshWeatherAsync()
    {
        isLoading = true;
        StateHasChanged();

        try
        {
            var weather = await Http.GetFromJsonAsync<WeatherData>("api/weather");
            if (weather is not null)
            {
                State.SetWeather(weather);
            }
        }
        finally
        {
            isLoading = false;
        }
    }

    public void Dispose()
    {
        State.OnChange -= StateHasChanged;
        cts?.Cancel();
    }
}
```

---

## ▶️ Expected result

Navigate to `/dynamic-weather`. The page loads the JSONPlaceholder users on
first render. Clicking **Refresh Users** or **Refresh Weather** briefly shows the
*Loading…* indicator, then the corresponding section updates in place. Because
both the page and any other component read from the same singleton
`WeatherStateService`, every view that subscribes to `OnChange` re-renders the
moment the shared state changes — and rapid clicks cancel the previous request
instead of stacking up.

---

## ☑️ Definition of done

- [ ] `DynamicWeather.razor` exists in `Pages/` with the `/dynamic-weather` route.
- [ ] `Models/User.cs` and `Models/WeatherData.cs` model the two API responses.
- [ ] User data is fetched and deserialized in `OnInitializedAsync` with a `CancellationToken` that cancels overlapping requests.
- [ ] `Services/WeatherStateService.cs` stores both datasets and exposes an `OnChange` event (`Action`/`EventCallback`).
- [ ] `builder.Services.AddSingleton<WeatherStateService>();` is registered in `Program.cs`.
- [ ] The component subscribes to `OnChange` and calls `StateHasChanged()` so the UI updates when state changes.
- [ ] Buttons trigger fresh fetches and a **loading indicator** shows while a call is in progress.

---

## 🔑 Key concepts

- **Typed JSON deserialization** — `HttpClient.GetFromJsonAsync<T>` (from `System.Net.Http.Json`) maps the response straight into `User`/`WeatherData`; `[JsonPropertyName]` bridges JSON casing to C# properties.
- **Cancelling stale requests** — a fresh `CancellationTokenSource` per call lets a newer request `Cancel()` the previous one, so out-of-order responses never overwrite newer data.
- **Shared state via a singleton service** — a single `WeatherStateService` registered with `AddSingleton` is the single source of truth, decoupling data fetching from the components that display it.
- **The notify pattern** — the service raises `OnChange` (an `Action`/`EventCallback`); components subscribe with `OnChange += StateHasChanged` and **must** unsubscribe in `Dispose` to prevent leaks.
- **Explicit re-rendering** — Blazor re-renders after its own event handlers automatically, but you call `StateHasChanged()` yourself to reflect changes triggered outside the normal lifecycle (the loading flag, the service event).
