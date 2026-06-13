# Analyzing a Public API

**Course 7 — Full-Stack Integration** · Module 2 · Lesson 4 · `You Try It!`

> Evaluate a public API through three lenses — **performance**, **security**, and
> **maintainability** — then turn those insights into concrete strategies for consuming the
> API from a **Blazor** app. You will study **OpenWeather** as a worked example, then pick your
> own API and write a short analysis report.

---

## 🎯 Objective

Evaluate the performance, security, and maintainability strategies of a public API and apply
those insights to improve a Blazor application's integration with that API.

---

## 📦 What you will produce

A short **analysis report (200–300 words)** on a public API of your choice, covering three
dimensions and, for each, a practical Blazor strategy:

| Dimension           | Question to answer                                              | Blazor strategy |
| ------------------- | -------------------------------------------------------------- | --------------- |
| **Rate limits**     | What are the API's limits, and how do you stay under them?     | Throttle / batch calls, limit refresh intervals |
| **Caching**         | Does the API recommend caching? How would you cache responses? | In-memory cache or browser local storage, short TTL |
| **Security**        | Does the API require authentication? How are credentials kept? | Store keys in `appsettings.json`, inject via DI, enforce HTTPS |
| **Maintainability** | How do you keep the integration clean and resilient?           | Centralize calls in a service class, handle errors (HTTP 400 / 429) |

**Flow:** `Pick an API  →  read its docs  →  analyze (limits · caching · security)  →  map each to a Blazor strategy  →  write the report`

---

## 🔎 Walkthrough — the OpenWeather case study

The **OpenWeather API** provides weather data for cities worldwide — current weather,
forecasts, and historical data. It is a well-documented public API, which makes it a good
model for how performance, security, and maintainability concerns map onto a Blazor
integration.

**Source:** OpenWeather API Documentation — <https://openweathermap.org/api>

### API at a glance

| Property        | Value |
| --------------- | ----- |
| **Endpoint**    | `https://api.openweathermap.org/data/2.5/weather` |
| **Rate limits** | Free tier: **60 calls per minute** |
| **Caching**     | Encourages response caching for improved performance |
| **Security**    | Requires an **API key** for authentication; **HTTPS** enforced |

### Rate limits and performance

The free tier enforces **60 requests per minute**. To stay under that limit in Blazor, reduce
unnecessary calls: limit data-refresh intervals (e.g. update weather **once per minute**), or
batch multiple requests. Reusing previously fetched results — i.e. client-side caching —
significantly improves performance and helps avoid exceeding the rate limit.

### Caching policies

OpenWeather encourages caching responses. In Blazor, weather data for a specific city can be
cached **in memory** or in **browser local storage** for a short period such as **five
minutes**. This minimizes redundant requests, reduces network usage, and improves
responsiveness — especially when users repeatedly request the same city.

### Security features

The API requires an **API key** and enforces **HTTPS**. The key should **never be hardcoded**.
Store it in configuration such as `appsettings.json` and read it through **dependency
injection**. HTTPS protects sensitive data — including the API key — in transit.

```jsonc
// appsettings.json — keep secrets out of source control (use user-secrets in dev)
{
  "OpenWeather": {
    "ApiKey": "<your-api-key>",
    "BaseUrl": "https://api.openweathermap.org/data/2.5/"
  }
}
```

### Maintainability

Centralize API interactions in a **reusable service class** so the code is easy to manage and
update, and add **error handling** for invalid requests (**HTTP 400**) and rate-limit
violations (**HTTP 429**). The example below shows how the strategies above come together in a
single, injectable Blazor service.

```csharp
// Program.cs — register a typed HttpClient + in-memory cache (DI + HTTPS base address)
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<WeatherService>(client =>
{
    var baseUrl = builder.Configuration["OpenWeather:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!); // HTTPS endpoint
});
```

```csharp
// WeatherService.cs — centralized API access: caching, secure key, error handling
public sealed class WeatherService
{
    private static readonly TimeSpan CacheFor = TimeSpan.FromMinutes(5); // short TTL

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly string _apiKey;

    public WeatherService(HttpClient http, IMemoryCache cache, IConfiguration config)
    {
        _http = http;
        _cache = cache;
        _apiKey = config["OpenWeather:ApiKey"]
                  ?? throw new InvalidOperationException("OpenWeather:ApiKey is not configured.");
    }

    public async Task<WeatherInfo?> GetWeatherAsync(string city, CancellationToken ct = default)
    {
        // Caching: serve from cache to respect the 60-calls/min limit
        if (_cache.TryGetValue(city, out WeatherInfo? cached))
            return cached;

        var url = $"weather?q={Uri.EscapeDataString(city)}&units=metric&appid={_apiKey}";

        try
        {
            var response = await _http.GetAsync(url, ct);

            if (response.StatusCode == HttpStatusCode.TooManyRequests) // HTTP 429
                throw new InvalidOperationException("Rate limit exceeded — try again shortly.");

            response.EnsureSuccessStatusCode(); // surfaces HTTP 400 and other failures

            var data = await response.Content.ReadFromJsonAsync<WeatherInfo>(cancellationToken: ct);
            if (data is not null)
                _cache.Set(city, data, CacheFor);

            return data;
        }
        catch (HttpRequestException ex)
        {
            // Maintainability: fail gracefully instead of crashing the component
            Console.Error.WriteLine($"Weather lookup failed for '{city}': {ex.Message}");
            return null;
        }
    }
}

public sealed record WeatherInfo(string Name, MainInfo Main);
public sealed record MainInfo(double Temp, int Humidity);
```

---

## 🧩 Your turn — evaluate and report on a public API

### 1. Select a public API

Choose one from the list below (or another well-documented public API):

| API             | Base URL |
| --------------- | -------- |
| **GitHub API**  | `https://api.github.com` |
| **OpenWeather** | `https://api.openweathermap.org` |
| **SpaceX API**  | `https://api.spacexdata.com/v4` |

### 2. Analyze the API

For your selected API, identify and describe:

- **Rate limits** — What are the API's rate limits, and how can you handle them in Blazor?
- **Caching policies** — Does the API recommend caching responses? How would you implement caching in Blazor?
- **Security features** — Does the API require authentication? How will you manage credentials securely?

### 3. Submit a report

Write a brief report (**200–300 words**) addressing the three points above. Include practical
strategies for applying these insights to a Blazor application — mirror the structure of the
OpenWeather walkthrough.

---

## 🌟 What good looks like

- [ ] One public API is chosen and its **documentation source is cited**
- [ ] **Rate limits** are stated as concrete numbers, with a Blazor handling strategy (throttle / batch / limit refresh)
- [ ] **Caching** is addressed — whether the API recommends it, plus a Blazor approach (in-memory or local storage) and a sensible TTL
- [ ] **Security** is covered — auth mechanism named, credentials kept out of code (config + DI), HTTPS enforced
- [ ] **Maintainability** is noted — centralized service class and error handling for **HTTP 400 / 429**
- [ ] The report is **200–300 words** and ties every insight back to a Blazor implementation

---

## 🔑 Key concepts

- **Read the contract first** — rate limits, caching guidance, and auth requirements all live in
  the API's own documentation; analyzing them up front shapes the whole integration.
- **Cache to respect limits** — short-TTL client-side caching (in-memory or local storage) cuts
  redundant calls, improves responsiveness, and is the simplest way to stay under a quota like 60/min.
- **Never hardcode credentials** — keep API keys in `appsettings.json` (or user-secrets), inject
  them via DI, and always call over HTTPS so the key is protected in transit.
- **Centralize and harden** — a single reusable service class with error handling for HTTP 400
  and 429 keeps the integration maintainable and resilient.
- **One pattern, many APIs** — performance, security, and maintainability are universal lenses;
  the same analysis transfers cleanly from OpenWeather to GitHub, SpaceX, or any public API.
