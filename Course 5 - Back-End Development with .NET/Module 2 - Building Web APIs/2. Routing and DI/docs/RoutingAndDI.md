# Routing and Dependency Injection

**Course 5 — Back-End Development with .NET** · Module 2 · Lesson 2 · `You Try It!`

> Build a minimal ASP.NET Core Web API that makes **dependency injection** visible. You
> register the *same* service contract under three different **lifetimes** —
> `Singleton`, `Scoped`, and `Transient` — then log a unique instance ID from
> middleware and from a route handler. Watching which IDs repeat tells you exactly
> when the container creates a new instance.

---

## 🎯 Objective

Implement dependency injection in an ASP.NET Core application by **creating services**,
**defining their lifetimes**, and **injecting them** into application components such as
middleware and minimal-API route handlers — and observe how each lifetime changes the
number of instances created per request.

---

## 🗂️ What you will build

A single-project Web API named **`DependencyInjectionDemo`**. Everything lives in
`Program.cs`: three service contracts, their implementations, the DI registrations, two
middleware components, and one root endpoint.

| Component                                                  | Lifetime    | What you should observe                                          |
| ---------------------------------------------------------- | ----------- | --------------------------------------------------------------- |
| `IMyServiceSingleton` → `MyServiceSingleton`               | `Singleton` | One instance ID for the **entire application** lifetime         |
| `IMyServiceScoped` → `MyServiceScoped`                     | `Scoped`    | One instance ID **per HTTP request** (shared across middleware) |
| `IMyServiceTransient` → `MyServiceTransient`               | `Transient` | A **new** instance ID every time it is resolved                 |

**Flow:** `HTTP request  →  Middleware 1  →  Middleware 2  →  GET /  →  joined log output`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (with the C# Dev Kit extension)
- Basic familiarity with the ASP.NET Core minimal-API hosting model

---

## 🛠️ Steps

### Step 1 — Prepare the application

Scaffold a new ASP.NET Core Web API project, move into it, and clear `Program.cs` so you
can build the structure up from scratch.

```bash
dotnet new webapi -n DependencyInjectionDemo
cd DependencyInjectionDemo
```

- Open `Program.cs` and **remove any existing content** — you will rewrite it completely.

### Step 2 — Create the service interfaces and implementations

Define a service contract and a class that implements it. Each implementation stores a
private `_serviceId` initialized in its field initializer with a **random six-digit
number** (`new Random().Next(100000, 999999)`), so a fresh instance always reports a new
ID. `LogCreation` writes a line to the console; `GetInfo` returns that same line so the
endpoint can include it in the HTTP response.

To compare lifetimes side by side, create **three** parallel contracts/classes —
`Singleton`, `Scoped`, and `Transient` — that differ only in the label they print.

```csharp
public interface IMyServiceSingleton
{
    void LogCreation(string source);
    string GetInfo(string source);
}

public interface IMyServiceScoped
{
    void LogCreation(string source);
    string GetInfo(string source);
}

public interface IMyServiceTransient
{
    void LogCreation(string source);
    string GetInfo(string source);
}

public class MyServiceSingleton : IMyServiceSingleton
{
    private readonly int _serviceId = new Random().Next(100000, 999999);

    public void LogCreation(string source)
    {
        Console.WriteLine(GetInfo(source));
    }

    public string GetInfo(string source)
    {
        return $"[Singleton | ID: {_serviceId}] → {source} | Explanation: Same instance across the whole application lifetime";
    }
}

public class MyServiceScoped : IMyServiceScoped
{
    private readonly int _serviceId = new Random().Next(100000, 999999);

    public void LogCreation(string source)
    {
        Console.WriteLine(GetInfo(source));
    }

    public string GetInfo(string source)
    {
        return $"[Scoped | ID: {_serviceId}] → {source} | Explanation: Same instance reused within one HTTP request";
    }
}

public class MyServiceTransient : IMyServiceTransient
{
    private readonly int _serviceId = new Random().Next(100000, 999999);

    public void LogCreation(string source)
    {
        Console.WriteLine(GetInfo(source));
    }

    public string GetInfo(string source)
    {
        return $"[Transient | ID: {_serviceId}] → {source} | Explanation: Always a new instance, even within the same request";
    }
}
```

### Step 3 — Register the services with their lifetimes

In the builder section, register each contract against its implementation using the
matching lifetime method. Calling `AddControllers()` keeps the standard MVC services
available even though this demo only uses a minimal-API endpoint.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IMyServiceSingleton, MyServiceSingleton>();
builder.Services.AddScoped<IMyServiceScoped, MyServiceScoped>();
builder.Services.AddTransient<IMyServiceTransient, MyServiceTransient>();

var app = builder.Build();
```

> `AddSingleton` creates one instance for the app, `AddScoped` one per request, and
> `AddTransient` one per resolution — this single difference drives the whole experiment.

### Step 4 — Add middleware that resolves the services

Add two middleware components **before** the endpoint. Because middleware is part of the
application pipeline (not an injectable handler), pull each service from
`context.RequestServices.GetRequiredService<T>()` and log a message from each.

```csharp
app.Use(async (context, next) =>
{
    context.RequestServices.GetRequiredService<IMyServiceSingleton>().LogCreation("Middleware 1");
    context.RequestServices.GetRequiredService<IMyServiceScoped>().LogCreation("Middleware 1");
    context.RequestServices.GetRequiredService<IMyServiceTransient>().LogCreation("Middleware 1");
    await next.Invoke();
});

app.Use(async (context, next) =>
{
    context.RequestServices.GetRequiredService<IMyServiceSingleton>().LogCreation("Middleware 2");
    context.RequestServices.GetRequiredService<IMyServiceScoped>().LogCreation("Middleware 2");
    context.RequestServices.GetRequiredService<IMyServiceTransient>().LogCreation("Middleware 2");
    await next.Invoke();
});
```

### Step 5 — Inject the services into the root endpoint

Map a `GET /` handler and let the framework **inject** all three services as parameters.
Collect each `GetInfo("Root endpoint")` line and return them joined, so the browser
response mirrors what the console logged.

```csharp
app.MapGet("/", (
    IMyServiceSingleton singleton,
    IMyServiceScoped scoped,
    IMyServiceTransient transient) =>
{
    var output = new List<string>();
    output.Add(singleton.GetInfo("Root endpoint"));
    output.Add(scoped.GetInfo("Root endpoint"));
    output.Add(transient.GetInfo("Root endpoint"));
    return string.Join("\n", output);
});

app.Run();
```

### Step 6 — Run and compare the lifetimes

Run the application, hit the root route a couple of times, and read the console output.

```bash
dotnet run
```

- Open the printed URL (or run `curl http://localhost:5000/`) and **refresh twice**.
- Watch the IDs across `Middleware 1`, `Middleware 2`, and `Root endpoint`:
  - **Singleton** — the same ID on every line, on every request.
  - **Scoped** — the same ID within one request, a different ID on the next request.
  - **Transient** — a different ID on (almost) every line, even inside one request.

> The complete `Program.cs` for this lab is the concatenation of the snippets in Steps 3,
> 4, 5, and 2 (in that order): builder + registrations, the two middleware components, the
> endpoint and `app.Run()`, followed by the interface and class definitions.

---

## ▶️ Expected result

Each request prints seven lines to the console — three from each middleware and three from
the endpoint — and the browser shows the endpoint's three lines:

```text
[Singleton | ID: 482915] → Middleware 1 | Explanation: Same instance across the whole application lifetime
[Scoped    | ID: 731220] → Middleware 1 | Explanation: Same instance reused within one HTTP request
[Transient | ID: 905471] → Middleware 1 | Explanation: Always a new instance, even within the same request
[Singleton | ID: 482915] → Middleware 2 | Explanation: ...
[Scoped    | ID: 731220] → Middleware 2 | Explanation: ...
[Transient | ID: 118364] → Middleware 2 | Explanation: ...
[Singleton | ID: 482915] → Root endpoint | Explanation: ...
[Scoped    | ID: 731220] → Root endpoint | Explanation: ...
[Transient | ID: 660093] → Root endpoint | Explanation: ...
```

The **Singleton** ID is identical everywhere, the **Scoped** ID is stable within the
request, and the **Transient** ID changes line to line — proving the container honors each
declared lifetime. (Actual ID numbers will differ on every run.)

---

## ☑️ Definition of done

- [ ] `DependencyInjectionDemo` Web API project created and `Program.cs` cleared
- [ ] Three interfaces and three implementations defined, each with a random `_serviceId`
- [ ] Services registered with `AddSingleton`, `AddScoped`, and `AddTransient`
- [ ] Two middleware components resolve the services via `context.RequestServices.GetRequiredService<T>()`
- [ ] `GET /` endpoint receives all three services through **constructor-style parameter injection**
- [ ] `dotnet run` shows Singleton IDs constant, Scoped IDs per-request, Transient IDs per-resolution

---

## 🔑 Key concepts

- **Service lifetimes decide instance count** — `Singleton` (one per app), `Scoped` (one
  per request), and `Transient` (one per resolution) are the three lifetimes the built-in
  container offers, and choosing the wrong one is a classic source of bugs.
- **Two ways to obtain a service** — minimal-API handlers receive services as **method
  parameters** (the framework injects them), while middleware reaches into
  `context.RequestServices` to resolve them manually from the request scope.
- **A `Scoped` service is bounded by the HTTP request** — every component participating in
  the same request shares one instance, which is why it is the default home for things like
  `DbContext`.
- **Prefer the interface, not the concrete type** — registering `IMyService` → `MyService`
  lets consumers depend on the abstraction, keeping the code testable and swappable.
- **Don't capture shorter lifetimes in longer ones** — injecting a `Scoped` (or
  `Transient`) service into a `Singleton` would pin a single instance for the app's life;
  resolve per-request dependencies from the request scope instead.
