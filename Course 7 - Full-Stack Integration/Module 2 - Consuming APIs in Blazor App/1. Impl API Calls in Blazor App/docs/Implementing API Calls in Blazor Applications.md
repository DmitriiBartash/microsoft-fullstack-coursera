# Implementing API Calls in Blazor Applications

**Course 7 — Full-Stack Integration** · Module 2 · Lesson 1 · `You Try It!`

> Build a **Blazor WebAssembly** app that fetches live weather data from a public REST
> API with `HttpClient`. You'll bind the JSON response to a typed model, render it in a
> Razor component, and handle failures gracefully with a `try`/`catch` and fallback UI.

---

## 🎯 Objective

Learn how to consume an external HTTP API from a Blazor WebAssembly component — injecting
`HttpClient`, deserializing the JSON response into a strongly-typed model inside
`OnInitializedAsync`, and **handling errors** so the UI never breaks when the call fails.

---

## 🗂️ What you'll build

A Blazor WebAssembly project named **`WeatherApp`** with a single weather component:

| File                  | Responsibility                                                       |
| --------------------- | ------------------------------------------------------------------- |
| `Pages/WeatherFetch.razor` | Inject `HttpClient`, call the API, bind the model, show errors  |
| `Models/WeatherData.cs`    | Typed model matching the WeatherAPI `current.json` response     |
| `Program.cs`               | Register `HttpClient` for dependency injection                  |

**Flow:** `WeatherFetch  →  HttpClient.GetFromJsonAsync()  →  WeatherData  →  rendered UI (or fallback)`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- A free **WeatherAPI** account and **API key** from [weatherapi.com](https://www.weatherapi.com)
- Internet access from the dev machine to reach `api.weatherapi.com`

---

## 🛠️ Steps

### Step 1 — Prepare the application

Scaffold a new Blazor WebAssembly project, move into it, and run it once to confirm it boots.

```bash
dotnet new blazorwasm -o WeatherApp
cd WeatherApp
dotnet run
```

Open the URL printed in the terminal and confirm the app loads. **Note the port** after
`localhost:` — you'll navigate to the weather page on that same port later.

### Step 2 — Sign up for a weather API and get an API key

You need an API key to fetch weather data from a public provider such as **WeatherAPI**.

- Go to the [WeatherAPI website](https://www.weatherapi.com).
- Create a free account by signing up with your email address.
- After signing in, open the **API Keys** section of your account dashboard.
- **Copy the API key** — you'll paste it into the request URL in Step 3.

### Step 3 — Configure `HttpClient` to make a GET request

Set up `HttpClient` to fetch weather data and bind it to a typed model.

First, register `HttpClient` for DI. In a default `blazorwasm` template this is already in
`Program.cs`; confirm the registration points at the app's base address:

```csharp
// Program.cs
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WeatherApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient for outbound API calls
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
```

Next, define a data model that matches the structure of the WeatherAPI `current.json`
response. Create **`Models/WeatherData.cs`**:

```csharp
// Models/WeatherData.cs
using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class WeatherData
{
    [JsonPropertyName("location")]
    public Location? Location { get; set; }

    [JsonPropertyName("current")]
    public Current? Current { get; set; }
}

public class Location
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}

public class Current
{
    [JsonPropertyName("temp_c")]
    public double TempC { get; set; }

    [JsonPropertyName("condition")]
    public Condition? Condition { get; set; }
}

public class Condition
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}
```

Now create the Razor component **`Pages/WeatherFetch.razor`**. Inject `HttpClient`, then
fetch the data from the API endpoint in `OnInitializedAsync`. Use the following API URL,
replacing `YOUR_API_KEY` with the key you copied in Step 2:

```
https://api.weatherapi.com/v1/current.json?key=YOUR_API_KEY&q=London
```

```razor
@* Pages/WeatherFetch.razor *@
@page "/weatherfetch"
@using WeatherApp.Models
@inject HttpClient Http

<h1>Current Weather</h1>

@if (weather is not null)
{
    <p><strong>@weather.Location?.Name, @weather.Location?.Country</strong></p>
    <p>Temperature: @weather.Current?.TempC °C</p>
    <p>Condition: @weather.Current?.Condition?.Text</p>
}
else
{
    <p>Loading…</p>
}

@code {
    private WeatherData? weather;

    protected override async Task OnInitializedAsync()
    {
        var url = "https://api.weatherapi.com/v1/current.json?key=YOUR_API_KEY&q=London";
        weather = await Http.GetFromJsonAsync<WeatherData>(url);
    }
}
```

`GetFromJsonAsync<T>` lives in `System.Net.Http.Json`, which ships with Blazor WebAssembly —
no extra package needed.

### Step 4 — Implement error handling

Make sure the app handles errors during the API call instead of crashing.

- Wrap the API call in a `try`/`catch` block.
- In the `catch` block, **log the error** to the console and **show a user-friendly message**.
- Use a **fallback strategy** — keep the UI usable (placeholder text) when the call fails.

```razor
@* Pages/WeatherFetch.razor — error-handling version *@
@page "/weatherfetch"
@using WeatherApp.Models
@inject HttpClient Http

<h1>Current Weather</h1>

@if (errorMessage is not null)
{
    <p style="color:red">@errorMessage</p>
    <p>Showing placeholder data while the weather service is unavailable.</p>
    <p>Temperature: -- °C</p>
}
else if (weather is not null)
{
    <p><strong>@weather.Location?.Name, @weather.Location?.Country</strong></p>
    <p>Temperature: @weather.Current?.TempC °C</p>
    <p>Condition: @weather.Current?.Condition?.Text</p>
}
else
{
    <p>Loading…</p>
}

@code {
    private WeatherData? weather;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        var url = "https://api.weatherapi.com/v1/current.json?key=YOUR_API_KEY&q=London";
        try
        {
            weather = await Http.GetFromJsonAsync<WeatherData>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Weather API error: {ex.Message}");
            errorMessage = "Unable to load weather data. Please try again later.";
        }
    }
}
```

### Step 5 — Test the API call

Test both the happy path and the error-handling path.

```bash
dotnet run
```

- Navigate to `http://localhost:5000/weatherfetch` in your browser. If needed, change `5000`
  to the correct port from Step 1.
- Test the app with a **valid** endpoint — confirm the location, temperature, and condition appear.
- Change the query to an **invalid** value (for example `q=New York` against a broken URL, or
  a bad key) and verify the **error message and fallback data** display instead of a crash.

---

## ▶️ Expected result

At `/weatherfetch` the page shows London's name, country, current temperature in °C, and the
condition text fetched live from WeatherAPI. When the request fails, the page shows a friendly
error message plus placeholder data — never a broken screen.

---

## ☑️ Definition of done

- [ ] `WeatherApp` Blazor WebAssembly project created and runs with `dotnet run`
- [ ] A WeatherAPI key obtained and placed in the request URL
- [ ] `HttpClient` registered in `Program.cs` and injected into the component
- [ ] `WeatherData` model matches the `current.json` response shape
- [ ] `Pages/WeatherFetch.razor` fetches data in `OnInitializedAsync` and renders it
- [ ] `try`/`catch` logs the error and shows a friendly message with fallback data
- [ ] Valid and invalid endpoints both behave correctly at `/weatherfetch`

---

## 🔑 Key concepts

- **`HttpClient` via DI** — Blazor WebAssembly registers `HttpClient` in `Program.cs`; inject
  it into components with `@inject` rather than `new`-ing one up, so it's managed and reusable.
- **Typed JSON binding** — `GetFromJsonAsync<T>` deserializes the response straight into a
  model; `[JsonPropertyName]` maps snake_case API fields (`temp_c`) onto C# properties.
- **`OnInitializedAsync`** — the right lifecycle hook for first-load async data; the UI shows a
  loading state until the awaited call returns and `StateHasChanged` re-renders.
- **Graceful degradation** — wrapping the call in `try`/`catch` and rendering fallback UI keeps
  the page usable when the network or the upstream API fails.
- **Keys are secrets** — an API key in the URL is fine for a learning lab, but in production it
  belongs in configuration or a backend proxy, never committed to source control.
