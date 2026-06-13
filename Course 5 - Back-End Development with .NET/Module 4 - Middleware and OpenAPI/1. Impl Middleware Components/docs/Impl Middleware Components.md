# Implement Middleware Components

**Course 5 — Back-End Development with .NET** · Module 4 · Lesson 1 · `You Try It!`

> Build a small ASP.NET Core app and wire up its **middleware pipeline** end to end.
> You'll register built-in middleware (exception handling, authentication, authorization,
> HTTP logging), then write **custom middleware** that logs each request's path/status and
> measures its duration — and watch requests flow through the pipeline in order.

---

## 🎯 Objective

Set up and configure middleware components in an ASP.NET Core project: scaffold a new empty
app, plug in the essential **built-in** middleware, add your own **custom** middleware for
request logging and timing, and test the pipeline to see how requests and responses are
processed in sequence.

---

## 🗂️ What you will build

An ASP.NET Core empty web app named **`MyAspNetCoreApp`** whose entire pipeline lives in
`Program.cs`. The middleware is registered in a deliberate order — order is what defines the
pipeline.

| Stage | Middleware | Role |
| --- | --- | --- |
| 1 | Exception handling | `UseDeveloperExceptionPage()` in Development, `UseExceptionHandler("/error")` in Production |
| 2 | HTTP logging | `UseHttpLogging()` — captures request/response details |
| 3 | Authentication | `UseAuthentication()` — establishes the user's identity from the cookie |
| 4 | Authorization | `UseAuthorization()` — enforces policies such as `AdminsOnly` |
| 5 | Custom: request log | Logs `path`, `method`, and final `StatusCode` (colored output) |
| 6 | Custom: timing | A `Stopwatch` that reports request `Duration` in ms |
| 7 | Endpoints | `/`, `/login`, `/logout`, `/admin`, `/error` |

**Flow:** `Request → Exception handler → HTTP logging → Authentication → Authorization → Custom log → Custom timing → Endpoint → Response`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- A code editor such as Visual Studio Code (`code .`)
- A terminal and a web browser for testing

---

## 🛠️ Steps

### Step 1 — Prepare the application

Create a new ASP.NET Core **empty** project from the command line, move into it, and open it
in your editor.

```bash
dotnet new web -o MyAspNetCoreApp
cd MyAspNetCoreApp
code .
```

- In the project root, locate **`Program.cs`** — this single file is where every middleware
  component will be configured.

### Step 2 — Configure built-in middleware components

Register the essential built-in middleware: **exception handling**, **authentication**,
**authorization**, and **HTTP logging**. You don't have to fully implement auth — you just
need the middleware present in the pipeline. Add the HTTP logging *service* in
`builder.Services`, then apply each piece in the request pipeline after `builder.Build()`.

```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Built-in services
builder.Services.AddHttpLogging(o => { });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = null;
        o.AccessDeniedPath = null;
        // Return 403 instead of redirecting to a login page (this is an API-style app)
        o.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
        o.Events.OnRedirectToAccessDenied = ctx =>
        {
            ctx.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminsOnly", p => p.RequireRole("Admin"));
});

var app = builder.Build();

// Exception handling — environment dependent
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
```

- Save your changes after each configuration so you can verify everything is set up correctly.

### Step 3 — Develop custom middleware

Now add two pieces of custom middleware with `app.Use(...)`. The **first** logs the request
path and the final response status; the **second** wraps the rest of the pipeline in a
`Stopwatch` to record how long the request took. Place them *after* the built-in middleware so
they observe the fully-processed response.

```csharp
// Custom middleware #1 — log request path + response status
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var method = context.Request.Method;
    var time = DateTime.Now.ToString("HH:mm:ss.fff");

    await next.Invoke();

    var status = context.Response.StatusCode;
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("──────────────────────────────");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[{time}] {method} {path}");
    Console.ForegroundColor = status >= 400 ? ConsoleColor.Red : ConsoleColor.Yellow;
    Console.WriteLine($"Status: {status}");
    Console.ResetColor();

    if (path == "/admin" && status == 200)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Conclusion: Admin access granted.");
        Console.ResetColor();
    }
    else if ((path == "/admin" || path == "/logout") && status == 403)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Conclusion: Access denied. User not authorized.");
        Console.ResetColor();
    }
});

// Custom middleware #2 — measure request duration
app.Use(async (context, next) =>
{
    var watch = System.Diagnostics.Stopwatch.StartNew();
    await next.Invoke();
    watch.Stop();
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine($"Duration: {watch.ElapsedMilliseconds} ms");
    Console.WriteLine("──────────────────────────────\n");
    Console.ResetColor();
});
```

- Save the changes once you've completed all the configurations.

### Step 4 — Map the endpoints

Finally, map the routes that let you exercise the pipeline. `/login` signs in a `ClaimsPrincipal`
with the **Admin** role; `/logout` and `/admin` are guarded by the `AdminsOnly` policy; `/error`
is the production exception-handler target.

```csharp
app.MapGet("/", () => "Hello World");

// In practice this should be POST (but GET is used here for easy browser testing)
app.MapGet("/login", async (HttpContext ctx) =>
{
    var claims = new List<System.Security.Claims.Claim>
    {
        new(System.Security.Claims.ClaimTypes.Name, "TestUser"),
        new(System.Security.Claims.ClaimTypes.Role, "Admin")
    };
    var identity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new System.Security.Claims.ClaimsPrincipal(identity);
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    return Results.Ok("You are logged in as Admin.");
});

// In practice this should be POST (but GET is used here for easy browser testing)
app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok("You are logged out.");
}).RequireAuthorization("AdminsOnly");

app.MapGet("/admin", () => "Welcome, Admin!").RequireAuthorization("AdminsOnly");
app.MapGet("/error", () => Results.Problem("An error occurred"));

app.Run();
```

### Step 5 — Test the middleware pipeline

With all middleware in place, run the app and watch the pipeline process each request.

```bash
dotnet run
```

- Open a browser and make requests to the app, e.g. `http://localhost:5000`.
- Observe the terminal logs to confirm the output of the custom middleware, the request
  timing, and the built-in middleware.
- Try the secured routes: hit `/admin` **before** logging in (expect `403`), then visit
  `/login`, then `/admin` again (expect `200`).
- Verify that error handling redirects to `/error` in Production and shows detailed errors in
  Development.

---

## ▶️ Expected result

Each request produces a colored block in the terminal showing the path, method, final status
code, and elapsed time. For example:

```text
──────────────────────────────
[14:32:10.501] GET /
Status: 200
Duration: 8 ms
──────────────────────────────

──────────────────────────────
[14:32:10.585] GET /favicon.ico
Status: 404
Duration: 0 ms
──────────────────────────────
```

Hitting `/admin` while signed in adds `Conclusion: Admin access granted.`; hitting it while
unauthorized returns `403` and `Conclusion: Access denied. User not authorized.`

---

## ☑️ Definition of done

- [ ] `MyAspNetCoreApp` empty project created with `dotnet new web`
- [ ] Built-in middleware configured: exception handling (dev/prod), `UseHttpLogging`, `UseAuthentication`, `UseAuthorization`
- [ ] HTTP logging service registered via `builder.Services.AddHttpLogging`
- [ ] Custom middleware logs request `path`, `method`, and response `StatusCode`
- [ ] Custom middleware reports request `Duration` using a `Stopwatch`
- [ ] Endpoints `/`, `/login`, `/logout`, `/admin`, `/error` mapped, with `/logout` and `/admin` guarded by `AdminsOnly`
- [ ] `dotnet run` succeeds and the terminal shows per-request logs and timing

---

## 🔑 Key concepts

- **Order is the pipeline** — middleware runs in the order it's registered with `app.Use*`.
  Authentication must come before authorization, and your custom loggers sit after them so they
  can observe the final status code.
- **`next.Invoke()` splits before/after** — code before `await next()` runs on the way *in*;
  code after it runs on the way *out*. That's exactly how the timing middleware brackets the
  whole downstream pipeline with a `Stopwatch`.
- **Built-in vs. custom middleware** — `UseHttpLogging`, `UseAuthentication`, and
  `UseAuthorization` are framework components you register once; `app.Use(...)` lets you inject
  arbitrary per-request logic without writing a full middleware class.
- **Environment-aware error handling** — `UseDeveloperExceptionPage()` surfaces stack traces in
  Development, while `UseExceptionHandler("/error")` returns a clean response in Production.
- **Policy-based authorization** — `RequireAuthorization("AdminsOnly")` ties an endpoint to the
  `RequireRole("Admin")` policy; unauthorized requests are short-circuited to `403` before the
  endpoint ever runs.
