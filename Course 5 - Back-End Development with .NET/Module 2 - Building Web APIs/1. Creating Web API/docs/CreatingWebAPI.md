# Creating a Web API

**Course 5 — Back-End Development with .NET** · Module 2 · Lesson 1 · `You Try It!`

> Build a small **ASP.NET Core Web API** that manages an in-memory list of items with
> full **CRUD** (Create, Read, Update, Delete) operations using minimal-API endpoints,
> then exercise every endpoint with **Postman**.

---

## 🎯 Objective

By the end of this lab you will be able to set up a basic ASP.NET Core project, create a
web API, and implement **GET, POST, PUT, and DELETE** endpoints — and use **Postman** to
test your API.

---

## 🗂️ What you'll build

A minimal-API project (`ItemsApi`) that keeps a list of items in memory and exposes CRUD
endpoints under `/items`:

| Endpoint               | Verb     | Responsibility                          |
| ---------------------- | -------- | --------------------------------------- |
| `/`                    | `GET`    | Welcome message                         |
| `/items`               | `GET`    | Return all items                        |
| `/items/{id}`          | `GET`    | Return one item by `Id` (or `404`)      |
| `/items`               | `POST`   | Create an item, assign next `Id`        |
| `/items/{id}`          | `PUT`    | Update an existing item's `Name`        |
| `/items/{id}`          | `DELETE` | Remove an item by `Id`                  |

**Flow:** `Request  →  /items endpoint  →  in-memory List<Item>  →  Results (Ok / Created / NoContent / NotFound)`

---

## ✅ Prerequisites

- The **.NET SDK** installed — get the latest version from the official .NET website if needed
- **Visual Studio Code**
- **Postman** for testing the endpoints
- NuGet package: `Swashbuckle.AspNetCore` (added below; powers the Swagger UI used in `Program.cs`)

---

## 🛠️ Steps

### Step 1 — Prepare for the application

You'll create a small web API using ASP.NET Core that manages a simple list of items with
basic CRUD operations.

- Open **Visual Studio Code**.
- Make sure the **.NET SDK** is installed; if not, install the latest version from the
  official .NET website.
- Open the terminal in VS Code (`Ctrl + ~` on Windows/Linux or `Cmd + ~` on Mac).
- Create a new ASP.NET Core Web API project, move into it, add the Swagger package, and open it in VS Code:

```bash
dotnet new web -n ItemsApi
cd ItemsApi
dotnet add package Swashbuckle.AspNetCore
code .
```

> The `web` template gives you a minimal `Program.cs`. Adding `Swashbuckle.AspNetCore`
> provides the `AddSwaggerGen` / `UseSwaggerUI` APIs used in Step 3.

### Step 2 — Set up the API project

Set up the basic project structure and configure the components for your API.

- In `Program.cs`, remove any existing code and start with a clean slate.
- Use the following basic structure to set up your API routes:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Basic routes
app.MapGet("/", () => "Welcome to the Simple Web API!");

app.Run();
```

- Save the file and run your project.
- Open a web browser and check that the API is running.

### Step 3 — Implement CRUD endpoints

Create the endpoints for managing a list of items (GET, POST, PUT, DELETE).

- Create a new folder named `Models` and add a file called `Item.cs` defining a basic model:

```csharp
namespace ItemsApi.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

- In `Program.cs`, create an in-memory list to store items and implement all endpoints
  (GET all, GET by ID, POST, PUT, DELETE):

```csharp
using ItemsApi.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Items API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Items API v1");
    });
}

app.UseHttpsRedirection();

var items = new List<Item>();
var nextId = 1;

app.MapGet("/", () => "Welcome to the Simple Web API (NET 9)!");

var itemsApi = app.MapGroup("/items").WithOpenApi();

itemsApi.MapGet("/", () => items);

itemsApi.MapGet("/{id}", (int id) =>
{
    var item = items.FirstOrDefault(i => i.Id == id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});

itemsApi.MapPost("/", (Item newItem) =>
{
    newItem.Id = nextId++;
    items.Add(newItem);
    return Results.Created($"/items/{newItem.Id}", newItem);
});

itemsApi.MapPut("/{id}", (int id, Item updatedItem) =>
{
    var item = items.FirstOrDefault(i => i.Id == id);
    if (item is null) return Results.NotFound();
    item.Name = updatedItem.Name;
    return Results.NoContent();
});

itemsApi.MapDelete("/{id}", (int id) =>
{
    var item = items.FirstOrDefault(i => i.Id == id);
    if (item is null) return Results.NotFound();
    items.Remove(item);
    return Results.NoContent();
});

app.Run();
```

- Save the file and test the endpoints by running the project.

### Step 4 — Test the API with Postman

Use Postman to exercise each endpoint.

- Open **Postman** and create a new request.
- Set the request type (**GET, POST, PUT, DELETE**) in the dropdown menu.
- Enter the API URL in the request field, e.g. `http://localhost:5000/items`.
- For **POST** and **PUT** requests, open the **Body** tab, select **raw**, and choose **JSON**.
  Enter your JSON data, for example:

```json
{
    "name": "New Item"
}
```

- Click **Send** to make the request.
- Check the response in the lower section of Postman to confirm the API behaves as expected.

---

## ▶️ Expected result

The API runs locally and responds on every verb: `GET /items` returns the current list,
`POST /items` creates an item with an auto-assigned `Id` and returns `201 Created`, `PUT`
and `DELETE` return `204 No Content` on success, and unknown IDs return `404 Not Found` —
all verifiable from Postman.

---

## ☑️ Definition of done

- [ ] New ASP.NET Core project (`ItemsApi`) created and opened in VS Code
- [ ] `Program.cs` runs and serves the welcome message at `/`
- [ ] `Models/Item.cs` defines the `Item` model (`Id`, `Name`)
- [ ] All five `/items` endpoints implemented over the in-memory `List<Item>`
- [ ] Each verb (GET, POST, PUT, DELETE) verified in Postman with the expected status codes

---

## 🔑 Key concepts

- **Minimal APIs** — `MapGet`/`MapPost`/`MapPut`/`MapDelete` register endpoints directly on
  the `WebApplication`, and `MapGroup("/items")` keeps related routes together.
- **CRUD over HTTP verbs** — Create→`POST`, Read→`GET`, Update→`PUT`, Delete→`DELETE`, each
  mapped to the matching list operation.
- **Typed results** — `Results.Ok`, `Results.Created`, `Results.NoContent`, and
  `Results.NotFound` return the correct HTTP status codes for each outcome.
- **In-memory state** — a `List<Item>` with a `nextId` counter stands in for a database, so
  data resets every time the app restarts.
- **Test before you trust** — Postman lets you drive each endpoint with real requests and
  JSON bodies to confirm behavior independently of a UI.
