# Setting Up a Full-Stack Development Environment

**Course 7 — Full-Stack Integration** · Module 1 · Lesson 1 · `You Try It!`

> Stand up a complete full-stack workspace in Visual Studio Code: a **Blazor WebAssembly**
> front end and a **Minimal API** back end, scaffolded with the .NET CLI and wired together
> so the Blazor app fetches and renders data served by the API.

---

## 🎯 Objective

By the end of this activity, you will be able to set up a full-stack development environment
using **Visual Studio Code**, create front-end and back-end projects with **Blazor** and
**Minimal APIs**, and configure both projects to work seamlessly together.

---

## 🗂️ What you will build

Two .NET projects that run side by side and talk over HTTP:

| Project    | Template               | Command                          | Role                                            |
| ---------- | ---------------------- | -------------------------------- | ----------------------------------------------- |
| `frontend` | Blazor WebAssembly     | `dotnet new blazorwasm -o frontend` | UI that calls the API and renders the results   |
| `backend`  | Minimal API (Web API)  | `dotnet new webapi -o backend`      | Exposes a `/products` endpoint returning sample data |

**Flow:** `Blazor component  →  HttpClient.GetFromJsonAsync("/products")  →  Minimal API  →  Product[]  →  rendered list`

---

## ✅ Prerequisites

- **.NET SDK** installed — check with `dotnet --version`
- **Visual Studio Code**
- VS Code extensions: **C# Dev Kit**, **REST Client**, **CSharpier**

---

## 🛠️ Steps

### Step 1 — Prepare for the application

You will create a small application using the VS Code terminal you configured earlier. It
consists of a front-end Blazor project and a back-end Minimal API project.

1. Open Visual Studio Code and navigate to the folder where you want your projects.
2. Open the terminal with **`Ctrl + ~`**.
3. Create the front-end and back-end projects:

```bash
# Blazor WebAssembly front end -> ./frontend
dotnet new blazorwasm -o frontend

# Minimal API back end -> ./backend
dotnet new webapi -o backend
```

4. Open each project in its own VS Code window:

```bash
code frontend
code backend
```

### Step 2 — Install and verify dependencies

Set up and verify everything the projects need.

1. Confirm the SDK is available (if no version prints, reinstall the .NET SDK):

```bash
dotnet --version
```

2. Install the required VS Code extensions:

| Extension       | Purpose                                       |
| --------------- | --------------------------------------------- |
| **C# Dev Kit**  | Enhanced C# language support                  |
| **REST Client** | Make HTTP requests directly inside VS Code    |
| **CSharpier**   | Automatic code formatting                     |

3. Verify both projects compile — run `dotnet build` inside each folder:

```bash
cd frontend && dotnet build
cd ../backend && dotnet build
```

### Step 3 — Configure the front-end project

Prepare the Blazor project to display data retrieved from the back-end API. The
`blazorwasm` template already registers an `HttpClient` in `Program.cs` — point its
`BaseAddress` at the back-end API so components can call it.

`frontend/Program.cs`

```csharp
using frontend;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Point the front end at the back-end Minimal API.
// Replace the port with the HTTPS URL printed when the backend starts (Step 4).
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7001/")
});

await builder.Build().RunAsync();
```

Start the front end and confirm the default Blazor app loads in the browser:

```bash
dotnet watch
```

### Step 4 — Configure the back-end project

Set up the back end to handle API requests. Open `backend/Program.cs` and define a simple
endpoint that returns sample data — a list of products. Enable CORS so the Blazor app
(served from a different port) is allowed to call it.

`backend/Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allow the Blazor WASM origin to call this API during development.
builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

// Sample data endpoint consumed by the Blazor front end.
app.MapGet("/products", () => new[]
{
    new Product(1, "Keyboard", 49.99m),
    new Product(2, "Mouse", 24.50m),
    new Product(3, "Monitor", 199.00m)
});

app.Run();

record Product(int Id, string Name, decimal Price);
```

Start the back end and note the URL where the API is served (you will reuse the HTTPS port
in `frontend/Program.cs`):

```bash
dotnet watch
```

> Open `https://localhost:7001/products` in a browser — you should see the JSON array of products.

### Step 5 — Integrate the front-end and back-end projects

Connect the Blazor front end to the Minimal API. Use the injected `HttpClient` to call the
`/products` endpoint and render the results in a component.

Create `frontend/Pages/Products.razor`:

```razor
@page "/products"
@inject HttpClient Http

<h1>Products</h1>

@if (products is null)
{
    <p><em>Loading...</em></p>
}
else
{
    <ul>
        @foreach (var p in products)
        {
            <li>@p.Name — @p.Price.ToString("C")</li>
        }
    </ul>
}

@code {
    private Product[]? products;

    protected override async Task OnInitializedAsync()
    {
        products = await Http.GetFromJsonAsync<Product[]>("products");
    }

    private record Product(int Id, string Name, decimal Price);
}
```

With both `dotnet watch` sessions running, browse to `/products` in the Blazor app — the
component fetches the data from the API and displays the product list.

---

## ▶️ Expected result

Two projects run concurrently: the Minimal API serves `GET /products` (visible as JSON at
`https://localhost:7001/products`), and the Blazor `/products` page renders that same list in
the browser — confirming the front end and back end communicate end-to-end.

---

## ☑️ Definition of done

- [ ] `frontend` (Blazor WASM) and `backend` (Minimal API) projects scaffolded with the .NET CLI
- [ ] `dotnet --version` prints a version; **C# Dev Kit**, **REST Client**, and **CSharpier** installed
- [ ] `dotnet build` succeeds in both project folders
- [ ] `backend/Program.cs` exposes `GET /products` with CORS enabled for the front end
- [ ] `frontend/Program.cs` sets `HttpClient.BaseAddress` to the back-end URL
- [ ] The Blazor `/products` page renders data fetched from the API while both apps run under `dotnet watch`

---

## 🔑 Key concepts

- **Two-project full-stack layout** — front end and back end are independent .NET projects in
  separate folders/windows, each with its own `Program.cs`, started and reloaded independently.
- **Scaffolding with the .NET CLI** — `dotnet new blazorwasm` and `dotnet new webapi` generate
  runnable starting points; `dotnet build` verifies they compile before you add features.
- **Minimal APIs** — endpoints are declared directly in `Program.cs` with `app.MapGet(...)`,
  no controllers required, returning data that is serialized to JSON automatically.
- **HttpClient integration** — Blazor's registered `HttpClient` (with a `BaseAddress`) lets a
  component call the API via `GetFromJsonAsync`, the seam that joins the two tiers.
- **CORS in development** — because the two apps run on different ports (origins), the API must
  explicitly permit the front-end origin or the browser blocks the request.
