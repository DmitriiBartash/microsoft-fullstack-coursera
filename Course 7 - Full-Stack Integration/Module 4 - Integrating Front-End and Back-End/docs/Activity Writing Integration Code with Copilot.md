# Writing Integration Code with Copilot

**Course 7 — Full-Stack Integration** · Module 4 · `Activity`

> Build the first slice of **InventoryHub**: wire a **Blazor WebAssembly** front-end to a
> **Minimal API** back-end so the app can fetch and render a product list. You'll use
> **Microsoft Copilot** to generate the `HttpClient` call inside a Razor component, then refine
> it for error handling and readability. This is **Activity 1 of 4** — the integration you build
> here is the foundation for later debugging, JSON-structuring, and performance work.

---

## 🎯 Objective

Create the integration layer between a Blazor WebAssembly client and a Minimal API server: call
the `/api/products` endpoint from a Razor component with `HttpClient`, deserialize the JSON into a
strongly-typed `Product` model, and display the results — using Copilot to generate and refine the
code.

---

## 🗂️ What you will build

A solution named **`FullStackSolution`** containing two projects:

| Project       | Type                       | Responsibility                                                  |
| ------------- | -------------------------- | --------------------------------------------------------------- |
| `ClientApp`   | Blazor WebAssembly (`blazorwasm`) | Front-end UI; fetches and renders the product list      |
| `ServerApp`   | Minimal API (`webapi`)     | Exposes `GET /api/products` returning product JSON              |

The integration itself lives in one new Razor component, `FetchProducts.razor`, which owns the
`HttpClient` call and the `Product` model.

**Flow:** `FetchProducts.razor → HttpClient.GetFromJsonAsync("/api/products") → JSON → Product[] → rendered list`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code with the C# Dev Kit extension
- Access to **Microsoft Copilot** (or GitHub Copilot in VS Code) for code generation
- Basic familiarity with the terminal and running `dotnet` commands

---

## 🛠️ Steps

### Step 1 — Set up the base application and solution

Browse to the root folder where you want the app, then create the project folder and move into it:

```bash
mkdir FullStackApp
cd FullStackApp
```

Create the **Client** (Blazor WASM) and **Server** (Minimal API) applications:

```bash
dotnet new blazorwasm -n ClientApp
dotnet new webapi -n ServerApp
```

Create a solution and add both projects to it:

```bash
dotnet new sln -n FullStackSolution
dotnet sln add ClientApp ServerApp
```

Replace the contents of the **ServerApp's `Program.cs`** with this starter Minimal API. It exposes a
single `GET /api/products` endpoint returning two hard-coded products as JSON:

```csharp
// ServerApp/Program.cs — Minimal API back-end
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/products", () =>
{
    return new[]
    {
        new { Id = 1, Name = "Laptop", Price = 1200.50, Stock = 25 },
        new { Id = 2, Name = "Headphones", Price = 50.00, Stock = 100 }
    };
});

app.Run();
```

### Step 2 — Launch both applications

- Open **two** terminal windows in VS Code.
- In the first, run the client: `cd ClientApp` then `dotnet run`.
- In the second, run the server: `cd ServerApp` then `dotnet run`.
- Open the **ClientApp** (front end) in your browser — the data is **not** displayed yet because the
  integration code doesn't exist.
- Open the **ServerApp** (back end) and browse to `http://localhost:[port]/api/products` — confirm the
  API returns the product list in JSON.

> Note the server's port number from its terminal output — you'll point the client's `HttpClient` at it.

### Step 3 — Add the product list component

In the **ClientApp** project, create a file named **`FetchProducts.razor`** with this starter markup
and code-behind. The `OnInitializedAsync` method is intentionally empty — that's where the
integration logic goes:

```razor
@page "/fetchproducts"

<h3>Product List</h3>

<ul>
    @if (products != null)
    {
        foreach (var product in products)
        {
            <li>@product.Name - $@product.Price</li>
        }
    }
    else
    {
        <li>Loading...</li>
    }
</ul>

@code {
    private Product[]? products;

    protected override async Task OnInitializedAsync()
    {
        // API call logic will go here
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Stock { get; set; }
    }
}
```

### Step 4 — Generate the API call with Copilot

Ask **Copilot** to fill in `OnInitializedAsync` so it: (1) uses `HttpClient` to call `/api/products`,
and (2) deserializes the JSON response into the `Product` class. A clean, idiomatic result uses the
injected `HttpClient` and the `GetFromJsonAsync` helper:

```razor
@page "/fetchproducts"
@inject HttpClient Http
@using System.Net.Http.Json

<h3>Product List</h3>

<ul>
    @if (products != null)
    {
        foreach (var product in products)
        {
            <li>@product.Name - $@product.Price</li>
        }
    }
    else
    {
        <li>Loading...</li>
    }
</ul>

@code {
    private Product[]? products;

    protected override async Task OnInitializedAsync()
    {
        products = await Http.GetFromJsonAsync<Product[]>("/api/products");
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Stock { get; set; }
    }
}
```

For the request to resolve, the client's `HttpClient` base address must point at the **server's**
origin. In `ClientApp/Program.cs`, set it to the ServerApp URL from Step 2:

```csharp
// ClientApp/Program.cs — point HttpClient at the ServerApp
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5000") // use your ServerApp port
});
```

### Step 5 — Refine and test the integration

Use Copilot to refine the call so it's production-friendly — **add error handling** for invalid
responses or timeouts, and keep the code readable and maintainable:

```razor
@code {
    private Product[]? products;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            // GetFromJsonAsync throws on non-success status codes and deserializes in one call
            products = await Http.GetFromJsonAsync<Product[]>("/api/products");
        }
        catch (HttpRequestException ex)
        {
            errorMessage = $"Failed to load products: {ex.Message}";
            products = Array.Empty<Product>();
        }
        catch (Exception ex)
        {
            errorMessage = $"Unexpected error: {ex.Message}";
            products = Array.Empty<Product>();
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Stock { get; set; }
    }
}
```

Run both projects with `dotnet run` and browse to `/fetchproducts` in the client to verify the
product data appears. **Save your work** — Activity 2 expands this app to debug and resolve
integration issues (API routes, CORS, malformed JSON).

---

## ▶️ Expected result

Browsing to `/fetchproducts` in the ClientApp renders the live product list pulled from the Minimal
API:

```text
Product List
 • Laptop - $1200.5
 • Headphones - $50
```

The list briefly shows **Loading...** before the data arrives, and a failed request surfaces an error
message instead of a blank page.

---

## ☑️ Definition of done

- [ ] `FullStackSolution` created with `ClientApp` (Blazor WASM) and `ServerApp` (Minimal API) added
- [ ] `ServerApp/Program.cs` exposes `GET /api/products` returning product JSON
- [ ] `FetchProducts.razor` exists at route `/fetchproducts` with a strongly-typed `Product` model
- [ ] `OnInitializedAsync` calls the API via `HttpClient` and deserializes the response
- [ ] `HttpClient.BaseAddress` in the client points at the ServerApp origin
- [ ] Error handling wraps the call so timeouts / bad responses don't crash the UI
- [ ] Running both projects shows the product list at `/fetchproducts` in the browser

---

## 🔑 Key concepts

- **Front-end / back-end integration** — the client and server are separate processes; integration is
  an **HTTP call** from the Blazor component to the API endpoint, not a direct method call.
- **Typed deserialization** — `GetFromJsonAsync<Product[]>` fetches and maps JSON to C# objects in one
  step, so the UI binds to a strongly-typed model instead of raw text.
- **`HttpClient` base address** — a Blazor WASM client calling a *different* origin must have its
  `HttpClient.BaseAddress` (or absolute URLs) set to the server, or requests won't resolve.
- **Resilient calls** — wrapping the request in `try/catch` for `HttpRequestException` keeps the app
  responsive when the API is slow, down, or returns an error.
- **Copilot as a pair-programmer** — generate the first draft fast, then **refine** it for error
  handling, readability, and maintainability rather than shipping the raw suggestion.
