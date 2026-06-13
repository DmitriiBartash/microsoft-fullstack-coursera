# Project - Building a Simple API with Copilot

**Course 5 — Back-End Development with .NET** · Module 5 · `Activity`

> Use **Microsoft Copilot** to scaffold, write, and enhance a **User Management API**
> for TechHive Solutions. You will build an ASP.NET Core Web API named `UserManagementAPI`
> with full **CRUD** endpoints for user records, then test every route in Postman — the
> first of three activities that grow into a complete back-end project.

---

## 🎯 Objective

Practice AI-assisted back-end development: have Copilot generate the boilerplate and CRUD
endpoints for a `UserManagementAPI`, then **review, enhance, and test** the generated code so
you end with a working API and notes on exactly how Copilot helped.

---

## 🏢 Scenario

You've been hired by **TechHive Solutions** to develop a **User Management API** for their
internal tools. The HR and IT departments need an API that lets them **create, update,
retrieve, and delete** user records efficiently. Your task is to build the core functionality
of the API, using Microsoft Copilot to scaffold, enhance, and test the code.

---

## 🗂️ What you will build

A single ASP.NET Core Web API project named **`UserManagementAPI`** exposing CRUD over a
`User` resource:

| Method   | Route             | Purpose                                  |
| -------- | ----------------- | ---------------------------------------- |
| `GET`    | `/api/users`      | Retrieve the list of all users           |
| `GET`    | `/api/users/{id}` | Retrieve a single user by ID             |
| `POST`   | `/api/users`      | Add a new user                           |
| `PUT`    | `/api/users/{id}` | Update an existing user's details        |
| `DELETE` | `/api/users/{id}` | Remove a user by ID                      |

**Flow:** `Copilot scaffolds Program.cs  →  generate UsersController CRUD  →  run  →  test in Postman  →  document Copilot's help`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (or Visual Studio) with the **GitHub Copilot** extension enabled
- **Postman** (or a similar HTTP client) to exercise the endpoints

---

## 🛠️ Steps

### Step 1 — Review the scenario

Read the TechHive scenario above and confirm the intent: an internal API that performs CRUD on
user records. Each user has an **ID**, a **name**, and an **email** — keep the model small for
this first activity.

### Step 2 — Set up the project

Create a new ASP.NET Core Web API project named `UserManagementAPI`, then ask Copilot to scaffold
the startup wiring in `Program.cs`.

```bash
dotnet new webapi -n UserManagementAPI
cd UserManagementAPI
dotnet run
```

Open `Program.cs` and use a Copilot prompt such as *"scaffold a minimal ASP.NET Core Web API
host with controllers and Swagger enabled."* The result should look like this:

```csharp
// Program.cs — host setup: controllers + Swagger
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
```

### Step 3 — Define the model

Ask Copilot to generate a simple `User` record. Create `Models/User.cs`:

```csharp
namespace UserManagementAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

### Step 4 — Generate API endpoints

Prompt Copilot to *"create a UsersController with CRUD endpoints backed by an in-memory list."*
Create `Controllers/UsersController.cs` and enhance Copilot's output into the following:

```csharp
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    // In-memory store for this first activity (replaced by a database later).
    private static readonly List<User> Users = new();
    private static int _nextId = 1;

    // GET /api/users — retrieve all users
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAll() => Ok(Users);

    // GET /api/users/{id} — retrieve a specific user
    [HttpGet("{id:int}")]
    public ActionResult<User> GetById(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        return user is null ? NotFound() : Ok(user);
    }

    // POST /api/users — add a new user
    [HttpPost]
    public ActionResult<User> Create(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email))
            return BadRequest("Name and Email are required.");

        user.Id = _nextId++;
        Users.Add(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    // PUT /api/users/{id} — update an existing user
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, User updated)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user is null) return NotFound();

        user.Name = updated.Name;
        user.Email = updated.Email;
        return NoContent();
    }

    // DELETE /api/users/{id} — remove a user
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user is null) return NotFound();

        Users.Remove(user);
        return NoContent();
    }
}
```

### Step 5 — Test API functionality

Run the API and exercise every endpoint with Postman (or the Swagger UI at `/swagger`).

```bash
dotnet run
```

Test each CRUD route, for example:

```http
POST https://localhost:5001/api/users
Content-Type: application/json

{ "name": "Ada Lovelace", "email": "ada@techhive.io" }
```

As you go, **document the specific ways Copilot assisted** — which prompts produced the
boilerplate, the controller, and the validation, and what you had to refine by hand.

### Step 6 — Save your work

Confirm you have a functional `UserManagementAPI` with robust CRUD operations plus your notes on
Copilot's contribution, then save the project. **You will reuse this code in later activities.**

---

## ▶️ Expected result

Running `dotnet run` starts the API; in Postman every endpoint behaves correctly —
`POST` creates a user and returns **201 Created**, `GET` lists or fetches users,
`PUT` updates a record (**204**), `DELETE` removes it (**204**), and unknown IDs return
**404 Not Found**.

---

## ☑️ Definition of done

- [ ] `UserManagementAPI` Web API project created with Copilot-scaffolded `Program.cs`
- [ ] `User` model defined with `Id`, `Name`, and `Email`
- [ ] `UsersController` exposes all five CRUD endpoints (`GET` list, `GET` by id, `POST`, `PUT`, `DELETE`)
- [ ] Basic input validation and correct status codes (201 / 204 / 404) in place
- [ ] Every endpoint tested in Postman or Swagger
- [ ] Notes recorded on how Copilot assisted, and the project saved for later activities

---

## 🔑 Key concepts

- **AI-assisted scaffolding** — Copilot accelerates boilerplate and CRUD generation, but you stay
  the author: read, refine, and verify every block it produces.
- **RESTful CRUD design** — map HTTP verbs to actions (`GET`/`POST`/`PUT`/`DELETE`) and return
  meaningful status codes (`200`, `201`, `204`, `404`) rather than always `200`.
- **Iterative prompting** — clear, specific prompts ("controller with in-memory store and
  validation") yield better generated code than vague ones; refine the prompt, not just the output.
- **Test before you trust** — generated endpoints aren't done until exercised end-to-end in
  Postman/Swagger across happy-path and error cases.
- **Build incrementally** — this activity establishes the API core that later activities extend
  (persistence, middleware, auth), so keep the model and structure clean.
