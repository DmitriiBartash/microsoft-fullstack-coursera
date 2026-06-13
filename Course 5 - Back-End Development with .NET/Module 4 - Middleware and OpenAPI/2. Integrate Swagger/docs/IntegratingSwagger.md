# Integrating Swagger and Generating API Clients

**Course 5 — Back-End Development with .NET** · Module 4 · Lesson 2 · `You Try It!`

> Stand up a minimal ASP.NET Core Web API, expose its contract through **Swagger / OpenAPI**,
> then point **NSwag** at that contract to **generate a strongly-typed C# API client** — and
> finally consume that generated client from the same app to call the endpoint end-to-end.

---

## 🎯 Objective

Integrate Swagger with an ASP.NET Core application and generate API clients with NSwag. You'll
practice **configuring Swagger middleware**, **generating client code** from the live OpenAPI
document, and **customizing** the generated namespace and class name.

---

## 🗂️ What you will build

A console-hosted Web API project named **`SwaggerApiClientLab`** made of these files:

| File                   | Responsibility                                                        |
| ---------------------- | --------------------------------------------------------------------- |
| `Program.cs`           | Host the API, register + serve Swagger, then drive the client         |
| `Controllers/UserController.cs` | Define the `User` model and the `GetUser` endpoint           |
| `ClientGenerator.cs`   | Fetch `swagger.json` and emit a strongly-typed client via NSwag       |
| `GeneratedApiClient.cs`| **Generated** output — the `CustomApiClient` your app calls           |

**Flow:** `Program → AddSwaggerGen / UseSwagger → swagger.json → NSwag CSharpClientGenerator → GeneratedApiClient.cs → call GetUser`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code with an integrated terminal
- NuGet packages: `Swashbuckle.AspNetCore`, `NSwag.Core`, `NSwag.CodeGeneration.CSharp`, `NSwag.ApiDescription.Client`

---

## 🛠️ Steps

### Step 1 — Set up a new console application

Create the project, move into it, add the Swagger + NSwag packages, and add a `Controllers` folder.

```bash
dotnet new console -o SwaggerApiClientLab
cd SwaggerApiClientLab
dotnet add package Swashbuckle.AspNetCore
dotnet add package NSwag.Core
dotnet add package NSwag.CodeGeneration.CSharp
dotnet add package NSwag.ApiDescription.Client
```

Create a folder named `Controllers` to hold your API controller, then confirm the structure:

```text
SwaggerApiClientLab
├── Controllers
├── Program.cs
└── SwaggerApiClientLab.csproj
```

> The console template ships with a top-level `Program.cs`. You'll replace its contents in Step 2
> to host a `WebApplication`, so the project behaves as a self-hosted Web API.

### Step 2 — Configure Swagger in the application

In `Program.cs`, build a `WebApplication`, register the controllers and Swagger generator, and
serve the API documentation. Swagger UI exposes the document at `/swagger/v1/swagger.json`.

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
```

Run the application and open the Swagger UI endpoint in your browser to verify the setup.

```bash
dotnet run
```

> The console prints the bound URL (for example `http://localhost:5000`). Browse to
> `/swagger` for the UI, or `/swagger/v1/swagger.json` for the raw OpenAPI document.

### Step 3 — Create the API specification

Inside `Controllers`, add `UserController.cs`. Define a simple `User` class with `Id` and `Name`
properties, and a `GetUser` endpoint that accepts a user ID and returns a sample `User`. The
`[ProducesResponseType]` attribute makes the response type explicit in the generated OpenAPI document.

```csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), 200)]
    public ActionResult<User> GetUser(int id)
    {
        return Ok(new User { Id = id, Name = $"User{id}" });
    }
}

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
```

Run the application again and view the Swagger documentation to confirm the `GET /api/User/{id}`
endpoint appears correctly.

### Step 4 — Generate client code with NSwag

Create `ClientGenerator.cs` in the project root. It fetches the live `swagger.json`, parses it into
an `OpenApiDocument`, and uses `CSharpClientGenerator` to emit a strongly-typed client file. The
generator's settings set the **class name** and **namespace** of the generated client.

```csharp
using NSwag;
using NSwag.CodeGeneration.CSharp;

public class ClientGenerator
{
    private readonly string _baseUrl;

    public ClientGenerator(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task GenerateClient()
    {
        using var httpClient = new HttpClient();
        var swaggerUrl = $"{_baseUrl}/swagger/v1/swagger.json";
        var swaggerJson = await httpClient.GetStringAsync(swaggerUrl);
        var document = await OpenApiDocument.FromJsonAsync(swaggerJson);

        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = "GeneratedApiClient",
            CSharpGeneratorSettings = { Namespace = "MyApiClientNamespace" }
        };

        var generator = new CSharpClientGenerator(document, settings);
        var code = generator.GenerateFile();
        await File.WriteAllTextAsync("GeneratedApiClient.cs", code);
        Console.WriteLine($"Client code generated from {swaggerUrl}");
    }
}
```

In `Program.cs`, drive the generator so it produces the client file. Because the document is only
served while the host is running, start the server asynchronously, wait for it to come up, then
generate:

- Replace `app.Run();` with `Task.Run(() => app.RunAsync());` so the server runs in the background.
- Below it, `await` a delay of **3 seconds or more** to let the server start.
- Below the delay, `await` the `GenerateClient` method on `ClientGenerator`.

Run the application again to generate the client code, and confirm `GeneratedApiClient.cs` appears
in the project directory.

### Step 5 — Customize the generated client code

Open `ClientGenerator.cs` and locate the class name and namespace settings. Change them to your own
preferences — for this lab, set the class name to `CustomApiClient` and the namespace to `CustomNamespace`.

```csharp
var settings = new CSharpClientGeneratorSettings
{
    ClassName = "CustomApiClient",
    CSharpGeneratorSettings = { Namespace = "CustomNamespace" }
};
```

Run the generator again and confirm the regenerated `GeneratedApiClient.cs` reflects the custom
settings. Then, in `Program.cs`, **comment out or remove the call to `GenerateClient`** — the client
is generated, so it no longer needs to run on every start.

### Step 6 — Integrate the client code into the application

Use the generated client to make a real API request from your app. Instantiate the client class with
the API base URL and an `HttpClient`, call the `GetUser` endpoint, and display the returned user data.

```bash
dotnet run
```

Run the application to confirm the API client integration works end-to-end.

---

## 📄 Final source files

The complete, runnable versions of the three hand-written files. `Program.cs` self-bootstraps:
on first run it generates `GeneratedApiClient.cs` and asks you to restart; on the next run it
loads the generated `CustomApiClient` and calls the endpoint.

### `Controllers/UserController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), 200)]
    public ActionResult<User> GetUser(int id)
    {
        return Ok(new User { Id = id, Name = $"User{id}" });
    }
}

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
```

### `ClientGenerator.cs`

```csharp
using NSwag;
using NSwag.CodeGeneration.CSharp;

public class ClientGenerator
{
    private readonly string _baseUrl;

    public ClientGenerator(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task GenerateClient()
    {
        using var httpClient = new HttpClient();
        var swaggerUrl = $"{_baseUrl}/swagger/v1/swagger.json";
        var swaggerJson = await httpClient.GetStringAsync(swaggerUrl);
        var document = await OpenApiDocument.FromJsonAsync(swaggerJson);

        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = "CustomApiClient",
            CSharpGeneratorSettings = { Namespace = "CustomNamespace" }
        };

        var generator = new CSharpClientGenerator(document, settings);
        var code = generator.GenerateFile();
        await File.WriteAllTextAsync("GeneratedApiClient.cs", code);
        Console.WriteLine($"Client code generated from {swaggerUrl}");
    }
}
```

### `Program.cs`

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();

        _ = Task.Run(async () => await app.RunAsync());
        await Task.Delay(3000);

        if (!File.Exists("GeneratedApiClient.cs"))
        {
            var url = app.Urls.FirstOrDefault() ?? "http://localhost:5000";
            var generator = new ClientGenerator(url);
            await generator.GenerateClient();
            Console.WriteLine("GeneratedApiClient.cs created. Please restart the application.");
            return;
        }

        await RunClient(app);
    }

    private static async Task RunClient(WebApplication app)
    {
        var url = app.Urls.FirstOrDefault() ?? "http://localhost:5000";
        var assembly = Assembly.GetExecutingAssembly();
        var clientType = assembly.GetType("CustomNamespace.CustomApiClient");
        if (clientType == null)
        {
            Console.WriteLine("Client type not found. Please rebuild the project.");
            return;
        }

        using var http = new HttpClient();
        var client = Activator.CreateInstance(clientType, url, http);
        if (client == null)
        {
            Console.WriteLine("Failed to create client instance.");
            return;
        }

        var method = clientType.GetMethod("GetUserAsync", new[] { typeof(int), typeof(CancellationToken) })
                     ?? clientType.GetMethod("UserAsync", new[] { typeof(int) });
        if (method == null)
        {
            Console.WriteLine("API method not found in generated client.");
            return;
        }

        var parameters = method.GetParameters().Length == 2
            ? new object[] { 1, CancellationToken.None }
            : new object[] { 1 };
        var task = method.Invoke(client, parameters) as Task;
        if (task == null)
        {
            Console.WriteLine("Failed to invoke client method.");
            return;
        }

        await task.ConfigureAwait(false);
        var resultProperty = task.GetType().GetProperty("Result");
        if (resultProperty != null)
        {
            var user = resultProperty.GetValue(task);
            var idProp = user?.GetType().GetProperty("Id");
            var nameProp = user?.GetType().GetProperty("Name");
            Console.WriteLine($"User ID: {idProp?.GetValue(user)}, User Name: {nameProp?.GetValue(user)}");
        }
        else
        {
            Console.WriteLine("Request completed successfully (200 OK).");
        }
    }
}
```

---

## ▶️ Expected result

On the **first** `dotnet run`, the server starts, NSwag reads `swagger.json`, writes
`GeneratedApiClient.cs`, and prints a message to restart. On the **second** run, the app loads the
generated `CustomApiClient`, calls `GET /api/User/1`, and prints something like:

```text
User ID: 1, User Name: User1
```

---

## ☑️ Definition of done

- [ ] `SwaggerApiClientLab` project created with the four Swagger/NSwag packages added
- [ ] `Program.cs` registers `AddSwaggerGen()` and serves `UseSwagger()` / `UseSwaggerUI()`
- [ ] Swagger UI shows `GET /api/User/{id}` at `/swagger/v1/swagger.json`
- [ ] `ClientGenerator.cs` fetches the OpenAPI document and writes `GeneratedApiClient.cs`
- [ ] Generated client uses the custom `CustomApiClient` class in `CustomNamespace`
- [ ] App consumes the generated client and prints the user returned by `GetUser`

---

## 🔑 Key concepts

- **OpenAPI as a contract** — `AddSwaggerGen` + `UseSwagger` reflect over your controllers to emit a
  machine-readable `swagger.json` that fully describes routes, parameters, and response shapes.
- **Spec-driven client generation** — NSwag's `CSharpClientGenerator` turns that document into a
  strongly-typed client, so callers get compile-time-checked methods instead of hand-written HTTP plumbing.
- **Generate against a live document** — the OpenAPI JSON is served by the running host, so the
  generator must start the server (`RunAsync`) and wait before fetching `swagger.json`.
- **Customizable output** — `ClassName` and `Namespace` on the generator settings control the shape of
  the generated code, letting you fit it to your project's conventions (`CustomApiClient` / `CustomNamespace`).
- **Generate once, then consume** — after the client file exists, the generation call is removed so the
  app simply instantiates the client and calls the API like any other typed service.
