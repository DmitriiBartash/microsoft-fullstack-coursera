# Error Handling and Logging in ASP.NET Core

**Course 5 — Back-End Development with .NET** · Module 2 · Lesson 3 · `You Try It!`

> Build a small ASP.NET Core web API and make it **resilient**: catch failures locally with
> `try`/`catch`, add a **global exception middleware** as a safety net, configure the built-in
> `ILogger`, then layer **Serilog** on top to write structured logs to the console *and* a
> rolling file. Finally, trigger errors on purpose and read the logs back.

---

## 🎯 Objective

By the end of this activity you will be able to implement error-handling mechanisms in an
ASP.NET Core web API, set up logging with both the **built-in** provider and a **third-party**
provider (**Serilog**), and capture and analyze the resulting log data.

---

## 🗂️ What you will build

A web API project named **`ErrorHandlingDemo`** with one diagnostic endpoint and a full
error-handling + logging pipeline.

| Piece                          | Responsibility                                                        |
| ------------------------------ | --------------------------------------------------------------------- |
| `ErrorHandlingController.cs`   | A `divide` endpoint that uses **`try`/`catch`** and logs each outcome |
| Global exception middleware    | Last-resort `try`/`catch` in `Program.cs` returning **HTTP 500**      |
| Built-in `ILogger`             | Per-class logging via dependency injection                            |
| **Serilog**                    | Structured sinks: **console + rolling file** (`logs/log-*.txt`)       |
| `appsettings.json`             | Log levels and Serilog sink configuration per environment             |

**Flow:** `Request → Global middleware → ErrorHandlingController.GetDivisionResult() → ILogger / Serilog → console + file`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- NuGet packages: `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `Serilog.Sinks.File`, `Swashbuckle.AspNetCore`

---

## 🛠️ Steps

### Step 1 — Prepare the application

Create a small ASP.NET Core web API in Visual Studio Code. It will expose a simple endpoint and
host the error-handling and logging configuration.

```bash
dotnet new webapi -n ErrorHandlingDemo
cd ErrorHandlingDemo
```

- Open `Program.cs` and **remove the default weather-forecast code**.
- Verify the environment is ready by confirming the project builds without errors:

```bash
dotnet build
```

### Step 2 — Basic error handling with try-catch blocks

Add error handling to manage unexpected situations inside an API endpoint.

- In the root of the project, create a new folder named **`Controllers`**.
- Inside it, create a file named **`ErrorHandlingController.cs`**.
- Define a class **`ErrorHandlingController`** that inherits from **`ControllerBase`**.
- Add an action method **`GetDivisionResult`** taking two parameters, `int numerator` and `int denominator`.
- Wrap the division in a **`try`/`catch`** block to handle any division errors.
- In the catch block, **log** the message *"Division by zero is not allowed."* and return a user-friendly error to the client.

```csharp
using Microsoft.AspNetCore.Mvc;

namespace ErrorHandlingDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorHandlingController : ControllerBase
    {
        private readonly ILogger<ErrorHandlingController> _logger;

        public ErrorHandlingController(ILogger<ErrorHandlingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("divide")]
        public IActionResult GetDivisionResult(int numerator, int denominator)
        {
            try
            {
                int result = numerator / denominator;
                _logger.LogInformation("Division succeeded: {Numerator} / {Denominator} = {Result}",
                    numerator, denominator, result);
                return Ok(new { Result = result });
            }
            catch (DivideByZeroException)
            {
                _logger.LogError("Division by zero is not allowed. Numerator: {Numerator}", numerator);
                return BadRequest(new { Error = "Division by zero is not allowed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during division.");
                return StatusCode(500, new { Error = "An unexpected server error occurred." });
            }
        }
    }
}
```

### Step 3 — Global error handling using middleware

ASP.NET Core allows centralized error handling with custom middleware. This catches **unhandled**
exceptions that slip past a controller.

- Open `Program.cs` and add middleware with **`app.Use`** to handle exceptions globally.
- Inside it, add a `try`/`catch` that catches any unhandled exception.
- Log a generic error message and set the response **`StatusCode` to 500**.
- Return a user-friendly message such as *"An unexpected error occurred. Please try again later."*

```csharp
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unhandled exception in global middleware.");
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            Error = "An unexpected error occurred. Please try again later."
        });
    }
});
```

### Step 4 — Setting up logging

Configure logging to capture application events and error detail.

- In `Program.cs`, configure logging by setting up **log levels** (e.g. `Error`, `Warning`, `Information`).
- Output to the **console** (the default ASP.NET Core host already wires the console provider; you can re-add it explicitly).
- Define log levels in **`appsettings.json`** for both **Development** and **Production** to control verbosity.

```csharp
// Built-in console logging (re-added explicitly for clarity)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
```

```jsonc
// appsettings.json — built-in Logging section
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

```jsonc
// appsettings.Development.json — more verbose in development
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Information"
  }
}
```

### Step 5 — Adding third-party logging (Serilog)

Third-party frameworks like **Serilog** add structured logging and richer sinks. Integrate it into the project.

- Install the Serilog packages:

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
```

- Configure Serilog to write to a **file** and the **console** via `appsettings.json`.
- Update `Program.cs` to use Serilog as the logging provider **before building the host** (`builder.Host.UseSerilog(...)`).

**`Program.cs` (complete):**

```csharp
using Serilog;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ErrorHandlingDemo API",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ErrorHandlingDemo API v1");
    });
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unhandled exception in global middleware.");
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            Error = "An unexpected error occurred. Please try again later."
        });
    }
});

app.MapControllers();
app.Run();
```

**`appsettings.json` (complete):**

```json
{
  "AllowedHosts": "*",
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Information",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

> **Note:** `WriteTo.Console()` in `Program.cs` is redundant with the `Console` sink declared in
> `appsettings.json`; either is enough. Keeping both is harmless, but configuration-only is cleaner.

### Step 6 — Capturing and analyzing log data

Test and analyze the generated logs to understand the application's behavior.

- Run the application and **intentionally trigger errors** (call `GetDivisionResult` with a denominator of zero):

```bash
dotnet run
```

```bash
# divide by zero → logged error + 400 response
curl "http://localhost:5000/api/ErrorHandling/divide?numerator=10&denominator=0"
```

- Open the console and verify the error logs contain appropriate messages and detail.
- Review the logged messages to confirm they match the format and detail level defined earlier.
- Inspect the rolling file under `logs/log-<date>.txt` for the persisted entries.
- Adjust log levels in `appsettings.json` if needed, then rerun to validate your changes.

---

## ▶️ Expected result

- `GET /api/ErrorHandling/divide?numerator=10&denominator=2` returns `{ "Result": 5 }` and logs an **Information** line.
- The same call with `denominator=0` returns **HTTP 400** `{ "Error": "Division by zero is not allowed." }` and logs an **Error** line.
- Any unhandled exception is caught by the global middleware, returns **HTTP 500** with a friendly message, and is logged.
- Identical, structured log entries appear in the **console** and in **`logs/log-<date>.txt`**.

---

## ☑️ Definition of done

- [ ] `ErrorHandlingDemo` web API project created and the default code removed
- [ ] `Controllers/ErrorHandlingController.cs` exposes `GET /api/ErrorHandling/divide` with a `try`/`catch`
- [ ] A `DivideByZeroException` returns **HTTP 400** with a user-friendly message and is logged
- [ ] Global `app.Use` middleware catches unhandled exceptions and returns **HTTP 500**
- [ ] Built-in log levels configured in `appsettings.json` (Development vs Production)
- [ ] Serilog packages added and `builder.Host.UseSerilog(...)` wired before `Build()`
- [ ] `dotnet run` + a divide-by-zero call produces matching entries in the console and `logs/log-<date>.txt`

---

## 🔑 Key concepts

- **Local vs global handling** — `try`/`catch` inside an action gives precise, user-friendly responses; global middleware is the **safety net** for anything that slips through, guaranteeing no raw stack trace reaches the client.
- **Right status, right message** — map failures to HTTP codes deliberately: `400` for bad input (divide by zero), `500` for unexpected server faults; never leak internal exception detail to callers.
- **Structured logging** — message templates like `"… {Numerator} / {Denominator} = {Result}"` capture *named properties*, not just text, so logs are queryable rather than flat strings.
- **Built-in + Serilog** — ASP.NET Core's `ILogger` abstraction stays the same; swapping in `UseSerilog` only changes the **provider**, adding sinks (rolling file) and config-driven levels without touching call sites.
- **Configuration over code** — log levels and sinks live in `appsettings.json`, so verbosity can change per environment with **no recompile**.
