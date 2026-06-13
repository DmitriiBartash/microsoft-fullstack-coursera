# Managing Roles and Claims in an ASP.NET Application

**Course 8 — Security and Authentication** · Module 1 · Lesson 3 · `You Try It!`

> Build a small ASP.NET Core **Web API** that authorizes requests two ways: by **role**
> (`Admin`) and by **claim** (`Department: IT`). You'll wire up ASP.NET Identity over an
> in-memory database, define **authorization policies**, and expose two endpoints that
> grant or deny access based on a manually-injected identity — then probe them in Postman.

---

## 🎯 Objective

By the end of this lab you will be able to implement **role-based** and **claims-based**
authorization in an ASP.NET Core Web API using ASP.NET Identity. You'll create a simple API
where user information is returned only when the caller carries the right role or claim.

---

## 🗂️ What you will build

A Web API project named **`RoleClaimsApp`** built from these pieces:

| File                       | Responsibility                                                          |
| -------------------------- | ---------------------------------------------------------------------- |
| `Data/ApplicationDbContext.cs` | Identity store — inherits `IdentityDbContext`, backed by an in-memory DB |
| `Program.cs`               | Register Identity, the DbContext, and the role/claims authorization policies |
| `Controllers/UsersController.cs` | Two endpoints: one gated by the **`Admin`** role, one by the **`Department: IT`** claim |

**Flow:** `Request → UsersController → ClaimsPrincipal (role / claim check) → 200 OK or 403 Forbidden`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- [Postman](https://www.postman.com/) for exercising the endpoints
- NuGet packages: `Microsoft.AspNetCore.Identity.EntityFrameworkCore` and `Microsoft.EntityFrameworkCore.InMemory`

---

## 🛠️ Steps

### Step 1 — Prepare the application

Create a new ASP.NET Core Web API project, move into it, and add the ASP.NET Identity
packages. The app will let users retrieve user information based on either **role-based**
or **claims-based** authorization.

```bash
dotnet new webapi -n RoleClaimsApp
cd RoleClaimsApp
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

- Remove any scaffolded sample files (e.g. the weather-forecast example) so that only
  `Program.cs` and `appsettings.json` remain to start from.
- Open `Program.cs` — you'll prepare it to register role-based and claims-based authorization in Step 3.

### Step 2 — Create the application `DbContext`

Configure ASP.NET Identity with an in-memory database by registering an
`ApplicationDbContext` that manages user and role data.

- Create a `Data` folder and add a new file **`Data/ApplicationDbContext.cs`**.
- Define an `ApplicationDbContext` class that inherits from `IdentityDbContext`.

```csharp
// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RoleClaimsApp.Data;

// Identity store wired to an in-memory provider for the lab
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
```

You'll register this context for Identity storage in `Program.cs` next.

### Step 3 — Configure ASP.NET Identity and authorization policies

Open `Program.cs` and:

1. Register ASP.NET Core services, including **controllers**.
2. Configure ASP.NET Identity with the **in-memory** database.
3. Add **role-based** and **claims-based** authorization policies.
4. Ensure the app can perform role and claim checks **without requiring authentication**
   (we inject the identity manually so endpoints stay reachable from Postman).

```csharp
// Program.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoleClaimsApp.Data;

var builder = WebApplication.CreateBuilder(args);

// MVC controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ASP.NET Identity backed by an in-memory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("RoleClaimsDb"));

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Authorization policies: one for the Admin role, one for the Department=IT claim
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("RequireITDepartment", policy =>
        policy.RequireClaim("Department", "IT"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
```

> Because we authorize manually inside the controller, we deliberately **do not** call
> `app.UseAuthentication()` — the endpoints remain anonymous so you can hit them directly
> from Postman, while the role/claim logic still decides what each one returns.

### Step 4 — Implement role-based authorization

Add an endpoint that returns a user **only if** the caller has the `Admin` role. To keep the
lab self-contained, the controller builds (injects) a `ClaimsPrincipal` manually to simulate
an authenticated admin, then checks the role.

- Create **`Controllers/UsersController.cs`**.
- Implement a `GET` endpoint that succeeds for an `Admin` and returns **403 Forbidden** otherwise.

```csharp
// Controllers/UsersController.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace RoleClaimsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // GET api/users/admin
    // Returns the user only when the (manually injected) identity has the Admin role.
    [HttpGet("admin")]
    public IActionResult GetAdminUser()
    {
        // Simulate an authenticated user that carries the Admin role
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "alice"),
            new Claim(ClaimTypes.Role, "Admin")
        }, authenticationType: "Manual");

        var user = new ClaimsPrincipal(identity);

        if (!user.IsInRole("Admin"))
        {
            return Forbid();
        }

        return Ok(new
        {
            Message = "Access granted: user has the Admin role.",
            User = user.Identity?.Name
        });
    }
}
```

### Step 5 — Implement claims-based authorization

Add a second endpoint that returns a user **only if** they carry a `Department: IT` claim.
Again, inject the identity manually to simulate an authenticated user with a `Department`
claim, then verify the claim.

```csharp
    // GET api/users/it
    // Returns the user only when the (manually injected) identity has Department=IT.
    [HttpGet("it")]
    public IActionResult GetItUser()
    {
        // Simulate an authenticated user that carries a Department claim
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "bob"),
            new Claim("Department", "IT")
        }, authenticationType: "Manual");

        var user = new ClaimsPrincipal(identity);

        var hasItClaim = user.HasClaim("Department", "IT");
        if (!hasItClaim)
        {
            return Forbid();
        }

        return Ok(new
        {
            Message = "Access granted: user has the Department=IT claim.",
            User = user.Identity?.Name
        });
    }
```

> Add this method **inside** the `UsersController` class from Step 4 (just below
> `GetAdminUser`). Flip the role or claim values to watch the endpoints fall through to
> `Forbid()` and return **403**.

### Step 6 — Test the endpoints with Postman

Start the application and exercise both endpoints — no authentication headers required.

```bash
dotnet run
```

- Open **Postman**.
- Send a `GET` to the **role-based** endpoint: `http://localhost:5000/api/users/admin`
  and observe the response.
- Send a `GET` to the **claims-based** endpoint: `http://localhost:5000/api/users/it`
  and observe the response.
- Confirm access is **granted** (200) or **denied** (403) according to the role and claim.

> Use the port shown in your console output (often `http://localhost:5000` or an
> `https://localhost:7xxx` URL). Swagger is also available at `/swagger` in Development.

---

## ▶️ Expected result

Both endpoints return **200 OK** with a success message, because the injected identities
satisfy their checks: `/api/users/admin` confirms the **Admin role** and `/api/users/it`
confirms the **`Department: IT`** claim. Remove the matching role or claim from the injected
identity and the same endpoint responds with **403 Forbidden**.

---

## ☑️ Definition of done

- [ ] `RoleClaimsApp` Web API project created with the Identity + EF Core InMemory packages added
- [ ] `Data/ApplicationDbContext.cs` inherits `IdentityDbContext` and is registered with the in-memory database
- [ ] `Program.cs` registers controllers, Identity, and the `RequireAdminRole` / `RequireITDepartment` policies
- [ ] `UsersController` exposes a **role-gated** endpoint (`/api/users/admin`) and a **claim-gated** endpoint (`/api/users/it`)
- [ ] Each endpoint returns success for a matching identity and **403 Forbidden** otherwise
- [ ] `dotnet run` serves the app and both endpoints respond correctly in Postman

---

## 🔑 Key concepts

- **Roles vs. claims** — a *role* is a coarse group label (`Admin`); a *claim* is a granular
  key/value statement about the user (`Department: IT`). Roles are really just a special claim,
  but modeling fine-grained access with claims scales better than multiplying roles.
- **Policy-based authorization** — `AddAuthorization` lets you name reusable rules
  (`RequireRole`, `RequireClaim`) once and apply them anywhere, instead of scattering literal
  role/claim checks across controllers.
- **`ClaimsPrincipal` is the identity** — every authorization decision reads from the
  principal's claims; `IsInRole(...)` and `HasClaim(...)` are simply queries over that claim set.
- **ASP.NET Identity over EF Core** — `IdentityDbContext` provides the user/role schema, and the
  in-memory provider gives a zero-setup store that's ideal for labs (swap it for SQL Server in production).
- **Manual identity injection is a teaching shortcut** — real apps authenticate first
  (`UseAuthentication`) so `HttpContext.User` is populated from a token or cookie; here we fabricate
  the principal to isolate the *authorization* logic.
