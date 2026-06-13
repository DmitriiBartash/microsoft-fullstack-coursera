# Designing and Securing Middleware Components

**Course 5 — Back-End Development with .NET** · Module 4 · Lesson 3 · `You Try It!`

> Build an ASP.NET Core Web API whose request pipeline is **just middleware** — no
> controllers. You'll layer five terminal/`Use` components that **enforce a simulated
> HTTPS rule**, block unauthorized paths, sanitize input, gate unauthenticated users,
> process work asynchronously, and **log every security event** — then prove each one
> with a targeted request.

---

## 🎯 Objective

Design, implement, and secure middleware components within an ASP.NET Core Web API so the
pipeline meets clear **performance** (async, short-circuiting) and **security** (HTTPS
enforcement, input validation, authentication, event logging) requirements.

---

## 🗂️ What you will build

A minimal Web API named **`MiddlewareOptimizationApp`** that listens on plain HTTP and runs
this ordered middleware chain in `Program.cs`:

| Order | Middleware                | Responsibility                                                        | Blocks with |
| ----- | ------------------------- | -------------------------------------------------------------------- | ----------- |
| 1     | Simulated HTTPS           | Require `?secure=true`; otherwise reject as non-HTTPS                 | `400`       |
| 2     | Unauthorized short-circuit| Stop any request to `/unauthorized` before it reaches the app        | `401`       |
| 3     | Input validation          | Reject `?input=` values containing HTML/script markup (`<...>`)       | `400`       |
| 4     | Authentication gate       | Require `?authenticated=true`; otherwise deny access                  | `403`       |
| 5     | Async processing          | `await` an I/O-style delay, then write the early response chunk       | —           |
| —     | Terminal endpoint         | `app.Map("/")` writes the final response                             | —           |

**Flow:** `Request → HTTPS check → /unauthorized check → input check → auth check → async stage → Map("/") → Response`

> *(The original includes a diagram of the middleware pipeline here.)*

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (with the integrated terminal)
- A way to send requests: a browser, **Postman**, or **`curl`**

---

## 🛠️ Steps

### Step 1 — Set up a new ASP.NET Core Web API project

Create a folder for the project, then scaffold and enter the Web API.

```bash
dotnet new webapi -o MiddlewareOptimizationApp
cd MiddlewareOptimizationApp
```

- Open `Program.cs` — this is the only file you will modify.
- Delete any controller files in the `Controllers` folder so the focus stays purely on middleware.

### Step 2 — Configure HTTP only

For simplicity, listen on **HTTP only** by removing the HTTPS-specific code from `Program.cs`.
This lets you test the middleware without a secure HTTPS connection. Configure Kestrel to
listen on `http://localhost:5294`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure to listen on HTTP only for simplicity
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5294);
});

var app = builder.Build();
```

This setup makes the app respond only to HTTP requests on `http://localhost:5294`.

### Step 3 — Design middleware for performance optimization and security

Write the middleware components that handle performance and security. Each `app.Use(...)`
runs in order and either **short-circuits** (writes a response and `return`s) or calls
`await next()` to pass control down the pipeline. Specifically, the chain must cover:

- **Simulated HTTPS enforcement** — use the `secure=true` query parameter to simulate HTTPS. If it is missing, block the request as if it were non-HTTPS.
- **Short-circuit unauthorized access** — stop further processing for unauthorized requests.
- **Asynchronous processing** — use `async` methods so I/O operations don't block other requests.
- **Input validation** — validate incoming request data and reject any unsafe input.
- **Authentication checks** — add an early authentication check to restrict unauthenticated users.
- **Security event logging** — log security events for any blocked or failed request.

The complete `Program.cs` ties every requirement together:

```csharp
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5294);
});

var app = builder.Build();

// 1. Simulated HTTPS enforcement — require ?secure=true
app.Use(async (context, next) =>
{
    if (!context.Request.Query.TryGetValue("secure", out var secure) || secure != "true")
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Simulated HTTPS Required");
        Console.WriteLine($"Security Event: {context.Request.Path} - Status Code: 400");
        return;
    }
    await next();
});

// 2. Short-circuit unauthorized access to /unauthorized
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/unauthorized"))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized Access");
        Console.WriteLine($"Security Event: {context.Request.Path} - Status Code: 401");
        return;
    }
    await next();
});

// 3. Input validation — reject HTML/script markup in ?input=
app.Use(async (context, next) =>
{
    if (context.Request.Query.TryGetValue("input", out var input))
    {
        if (Regex.IsMatch(input.ToString(), "<.*?>"))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid Input");
            Console.WriteLine($"Security Event: {context.Request.Path} - Status Code: 400");
            return;
        }
    }
    await next();
});

// 4. Authentication gate — require ?authenticated=true
app.Use(async (context, next) =>
{
    if (!context.Request.Query.TryGetValue("authenticated", out var authenticated) || authenticated != "true")
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Access Denied");
        Console.WriteLine($"Security Event: {context.Request.Path} - Status Code: 403");
        return;
    }
    await next();
});

// 5. Asynchronous processing — non-blocking I/O simulation
app.Use(async (context, next) =>
{
    await Task.Delay(100);
    await context.Response.WriteAsync("Processed Asynchronously\n");
    await next();
});

// Terminal endpoint
app.Map("/", async context =>
{
    await context.Response.WriteAsync("Final Response from Application");
});

app.Run();
```

### Step 4 — Test middleware performance and security

Run the app, then exercise each middleware with a targeted request. Use **Postman**, **`curl`**,
or paste the URLs directly into your browser.

```bash
dotnet run
```

| Test                            | URL                                                       | Expected output                                                | Status |
| ------------------------------- | --------------------------------------------------------- | -------------------------------------------------------------- | ------ |
| Async processing (happy path)   | `http://localhost:5294/?secure=true`                      | `Processed Asynchronously` then `Final Response from Application` | `200`  |
| Simulated HTTPS enforcement     | `http://localhost:5294/?secure=true&authenticated=true`   | `Simulated HTTPS Required`                                      | `400`  |
| Unauthorized access             | `http://localhost:5294/unauthorized?secure=true`          | `Unauthorized Access`                                          | `401`  |
| Invalid input                   | `http://localhost:5294/?secure=true&input=<script>`       | `Invalid Input`                                               | `400`  |
| Access denied (unauthenticated) | `http://localhost:5294/?secure=true`                      | `Access Denied`                                               | `403`  |
| Security event log              | Any request that returns `400`+                           | Console: `Security Event: /unauthorized - Status Code: 401`    | —      |

Because the middleware runs **in order**, a single URL can hit different checks depending on
which query parameters are present — e.g. `?secure=true` alone clears the HTTPS check but is
later denied (`403`) by the authentication gate.

---

## ▶️ Expected result

Each request lands on exactly the middleware meant to handle it: the happy path returns
`Processed Asynchronously` + `Final Response from Application`, while missing `secure`,
the `/unauthorized` path, unsafe `input`, and missing `authenticated` are each short-circuited
with their own status code — and **every blocked request prints a `Security Event:` line** to
the Visual Studio Code console.

---

## ☑️ Definition of done

- [ ] `MiddlewareOptimizationApp` Web API created and controller files removed
- [ ] Kestrel configured for **HTTP only** on `http://localhost:5294`
- [ ] `?secure=true` is required, otherwise `400 Simulated HTTPS Required`
- [ ] Requests to `/unauthorized` are short-circuited with `401`
- [ ] `?input=` containing `<...>` markup is rejected with `400 Invalid Input`
- [ ] `?authenticated=true` is required, otherwise `403 Access Denied`
- [ ] Async middleware writes `Processed Asynchronously` before the terminal response
- [ ] Every blocked request logs a `Security Event:` line to the console

---

## 🔑 Key concepts

- **Order is the contract** — middleware runs top-to-bottom, so placing HTTPS and auth checks *early* short-circuits bad requests before any expensive work runs.
- **Short-circuiting vs `next()`** — writing a response and `return`ing ends the pipeline; calling `await next()` passes control on. That choice *is* the security gate.
- **Async by default** — `await`ing I/O (`Task.Delay`, `WriteAsync`) frees the thread to serve other requests instead of blocking, which is the performance win.
- **Defense in depth** — independent layers for transport, input, and identity mean one weak check doesn't expose the whole app; each failure is also logged for auditing.
- **Simulation for learning** — query flags (`secure`, `authenticated`) stand in for real TLS and auth so you can focus on *pipeline behavior*; production would use real HTTPS redirection and an authentication scheme.
