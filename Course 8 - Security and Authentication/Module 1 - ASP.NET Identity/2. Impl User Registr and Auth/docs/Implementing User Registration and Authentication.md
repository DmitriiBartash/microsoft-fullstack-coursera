# Implementing User Registration and Authentication

**Course 8 — Security and Authentication** · Module 1 · Lesson 2 · `You Try It!`

> Build an ASP.NET Core MVC app that handles **user registration** and **authentication**
> with **ASP.NET Identity**, backed by the **Entity Framework Core in-memory provider**.
> Using in-memory storage lets you focus on the Identity workflow — `UserManager`,
> `SignInManager`, automatic password hashing — without standing up a real database.

---

## 🎯 Objective

By the end of this activity, you will implement a user registration and authentication
process using **ASP.NET Identity** in an ASP.NET Core application. You will use **in-memory
storage** instead of a database, allowing you to focus on functionality without requiring a
database setup.

---

## 🗂️ What you will build

An ASP.NET Core MVC project named **`UserAuthInMemoryApp`** with the following pieces:

| File                          | Responsibility                                                        |
| ----------------------------- | --------------------------------------------------------------------- |
| `Data/ApplicationDbContext.cs`| Identity database context (`IdentityDbContext`)                       |
| `Program.cs`                  | Register the in-memory DB, Identity services, and the auth middleware |
| `Models/RegisterViewModel.cs` | Email + password fields for the registration form                     |
| `Models/LoginViewModel.cs`    | Email, password, and **Remember Me** fields for the login form        |
| `Controllers/AccountController.cs` | `Register` / `Login` / `Logout` actions using Identity services  |
| `Views/Account/Register.cshtml`| The registration form                                                 |
| `Views/Account/Login.cshtml`  | The login form                                                        |

**Flow:** `Register form → UserManager.CreateAsync → Login form → SignInManager.PasswordSignInAsync → authenticated session`

---

## ✅ Prerequisites

- **.NET SDK** installed — check with `dotnet --version`
- **Visual Studio Code** with the **C# for Visual Studio Code** extension (powered by OmniSharp)
- Familiarity with running commands in the integrated **terminal**

---

## 🛠️ Steps

### Step 1 — Prepare for the application

You will create a new ASP.NET Core MVC application and add the Identity packages.

- Open Visual Studio Code and ensure the **C# for Visual Studio Code** extension is installed.
- Open the terminal with **Terminal > New Terminal**.
- Create a new ASP.NET Core MVC project — this creates a `UserAuthInMemoryApp` folder with the necessary project files:

```bash
dotnet new mvc -n UserAuthInMemoryApp
```

- Navigate into the project folder:

```bash
cd UserAuthInMemoryApp
```

- Add the required NuGet packages:

```bash
# ASP.NET Identity with EF Core
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

# ASP.NET Identity UI
dotnet add package Microsoft.AspNetCore.Identity.UI

# In-memory database provider
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

- Restore packages to ensure they are properly installed:

```bash
dotnet restore
```

### Step 2 — Create the application database context

You will define a custom database context that ASP.NET Identity uses for managing user data.
Create a new folder named **`Data`** and add a file named **`ApplicationDbContext.cs`**.

```csharp
// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UserAuthInMemoryApp.Data;

// Identity context: brings in Users, Roles, Claims, etc.
// IdentityUser is the built-in user type (Id, UserName, Email, PasswordHash, ...).
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
```

### Step 3 — Configure the application to use in-memory storage

You will configure the project to use the in-memory database and register Identity services.
Open **`Program.cs`** and replace its contents.

```csharp
// Program.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserAuthInMemoryApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Register the Identity DbContext using the in-memory provider.
// The named store lives only for the lifetime of the process.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("UserAuthInMemoryDb"));

// Register ASP.NET Identity, backed by ApplicationDbContext.
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        // Relax the defaults so the demo is easy to test.
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Order matters: authentication must run before authorization.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### Step 4 — Create the user registration form

You will create a registration form to let users input their email and password.

- In the **`Controllers`** folder, create **`AccountController.cs`** with a `Register` action that renders the form.
- In the **`Models`** folder, create **`RegisterViewModel.cs`** with properties for email and password.
- In the **`Views/Account`** folder, create **`Register.cshtml`** to display the form.

```csharp
// Models/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace UserAuthInMemoryApp.Models;

public class RegisterViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
```

```csharp
// Controllers/AccountController.cs  (registration GET action)
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserAuthInMemoryApp.Models;

namespace UserAuthInMemoryApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: render the empty registration form.
    [HttpGet]
    public IActionResult Register() => View();
}
```

```html
@* Views/Account/Register.cshtml *@
@model UserAuthInMemoryApp.Models.RegisterViewModel
@{
    ViewData["Title"] = "Register";
}

<h2>Register</h2>

<form asp-action="Register" method="post">
    <div asp-validation-summary="All" class="text-danger"></div>

    <div class="mb-3">
        <label asp-for="Email" class="form-label"></label>
        <input asp-for="Email" class="form-control" />
        <span asp-validation-for="Email" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Password" class="form-label"></label>
        <input asp-for="Password" class="form-control" />
        <span asp-validation-for="Password" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="ConfirmPassword" class="form-label"></label>
        <input asp-for="ConfirmPassword" class="form-control" />
        <span asp-validation-for="ConfirmPassword" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Register</button>
    <a asp-action="Login" class="btn btn-link">Already have an account? Log in</a>
</form>
```

### Step 5 — Handle user registration

You will process the form submission to register a new user.

- Update `AccountController` to handle the form **POST** and create a user with the `UserManager` service.
- Password hashing happens **automatically** via ASP.NET Identity.
- Redirect users to the **login** page upon successful registration.

```csharp
// Controllers/AccountController.cs  (registration POST action)

// POST: create the user. UserManager hashes the password for us.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    var user = new IdentityUser
    {
        UserName = model.Email,
        Email = model.Email
    };

    // CreateAsync stores the user and the *hashed* password in the in-memory store.
    var result = await _userManager.CreateAsync(user, model.Password);

    if (result.Succeeded)
        return RedirectToAction(nameof(Login));

    foreach (var error in result.Errors)
        ModelState.AddModelError(string.Empty, error.Description);

    return View(model);
}
```

### Step 6 — Implement user authentication

You will add login functionality to authenticate users.

- In the **`Models`** folder, create **`LoginViewModel.cs`** with properties for email, password, and **Remember Me**.
- Update `AccountController` with a login action that **renders** the form and **processes** logins (plus a logout action).
- In the **`Views/Account`** folder, create **`Login.cshtml`** to display the login form.

```csharp
// Models/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace UserAuthInMemoryApp.Models;

public class LoginViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}
```

```csharp
// Controllers/AccountController.cs  (login + logout actions)

// GET: render the empty login form.
[HttpGet]
public IActionResult Login() => View();

// POST: sign the user in via SignInManager.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    var result = await _signInManager.PasswordSignInAsync(
        model.Email,
        model.Password,
        isPersistent: model.RememberMe,   // honours "Remember Me"
        lockoutOnFailure: false);

    if (result.Succeeded)
        return RedirectToAction("Index", "Home");

    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Logout()
{
    await _signInManager.SignOutAsync();
    return RedirectToAction("Index", "Home");
}
```

```html
@* Views/Account/Login.cshtml *@
@model UserAuthInMemoryApp.Models.LoginViewModel
@{
    ViewData["Title"] = "Log in";
}

<h2>Log in</h2>

<form asp-action="Login" method="post">
    <div asp-validation-summary="All" class="text-danger"></div>

    <div class="mb-3">
        <label asp-for="Email" class="form-label"></label>
        <input asp-for="Email" class="form-control" />
        <span asp-validation-for="Email" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Password" class="form-label"></label>
        <input asp-for="Password" class="form-control" />
        <span asp-validation-for="Password" class="text-danger"></span>
    </div>

    <div class="form-check mb-3">
        <input asp-for="RememberMe" class="form-check-input" />
        <label asp-for="RememberMe" class="form-check-label"></label>
    </div>

    <button type="submit" class="btn btn-primary">Log in</button>
    <a asp-action="Register" class="btn btn-link">Create an account</a>
</form>
```

### Step 7 — Test the application

You will test the registration and authentication functionality.

```bash
dotnet run
```

- Open your browser and navigate to `https://localhost:[port number]` (the exact port is printed in the terminal), then go to `/Account/Register`.
- **Test registration:** fill out the registration form and submit it. Verify the new user is created (you are redirected to the login page).
- **Test login:** log in with the correct credentials, and try the **Remember Me** option.
- **Restart the application** and note that the data is **lost** — this is the expected behavior of in-memory storage.

---

## ▶️ Expected result

A registered user can log in successfully and lands back on the Home page in an authenticated
session. Logging in with wrong credentials shows an "Invalid login attempt." message. After
restarting the app, all previously registered users are gone, confirming that the in-memory
store does **not** persist between runs.

---

## ☑️ Definition of done

- [ ] `UserAuthInMemoryApp` MVC project created with the three Identity / EF Core packages added
- [ ] `ApplicationDbContext` derives from `IdentityDbContext<IdentityUser>`
- [ ] `Program.cs` registers `UseInMemoryDatabase`, `AddDefaultIdentity`, and `UseAuthentication`
- [ ] Registration creates a user via `UserManager.CreateAsync` (password hashed automatically)
- [ ] Login authenticates via `SignInManager.PasswordSignInAsync`, honouring **Remember Me**
- [ ] After `dotnet run`, a user can register, log in, and the data is lost on restart

---

## 🔑 Key concepts

- **ASP.NET Identity does the heavy lifting** — `UserManager` and `SignInManager` handle user
  creation, secure **password hashing**, and cookie-based sign-in, so you never store or compare
  raw passwords yourself.
- **In-memory storage is volatile** — the EF Core in-memory provider keeps data only for the
  process lifetime; it is ideal for demos and tests but loses everything on restart.
- **`AddDefaultIdentity` + `AddEntityFrameworkStores`** wire Identity to your `DbContext`, and
  Identity's password rules (length, character classes) are configurable in `Program.cs`.
- **Middleware order is load-bearing** — `UseAuthentication()` must come before
  `UseAuthorization()`, or the authenticated cookie is never read on incoming requests.
- **Remember Me = persistent cookie** — passing `isPersistent: true` to `PasswordSignInAsync`
  issues a cookie that survives the browser session, instead of an in-session cookie.
