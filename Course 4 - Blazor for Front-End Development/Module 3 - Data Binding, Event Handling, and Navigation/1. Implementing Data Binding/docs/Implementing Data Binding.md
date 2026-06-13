# Implementing Data Binding

**Course 4 — Blazor for Front-End Development** · Module 3 · Lesson 1 · `You Try It!`

> Build a **Recipe Manager** app in Blazor WebAssembly that ties together the three
> pillars of an interactive Blazor UI: **data binding** (lists and form inputs bound to
> C# state), **event handling** (`@onclick`, `EditForm` submits), and **routing /
> navigation** (route parameters and `NavigationManager`).

---

## 🎯 Objective

Create a Blazor WebAssembly application where users can **view a list of recipes**, **add
new recipes** through a validated form, and **open a detail page** for any recipe via a
dynamically routed URL — wiring the UI to state with one-way and two-way data binding and
driving navigation from code.

---

## 🗂️ What you will build

A Blazor WebAssembly project named **`RecipeManagerApp`** made of these pieces:

| File / Component        | Responsibility                                                        |
| ----------------------- | --------------------------------------------------------------------- |
| `Data/Recipe.cs`        | The model — `Id`, `Name`, `Description` (with validation attributes)  |
| `Data/RecipeService.cs` | In-memory store: `GetAll()`, `GetById(id)`, `Add(recipe)`             |
| `Pages/Home.razor`      | List all recipes; bind names/descriptions; link to details & add page |
| `Pages/AddRecipe.razor` | `EditForm` bound to a `Recipe`; submit adds and navigates home         |
| `Pages/RecipeDetails.razor` | Dynamic route `/recipe/{id:int}`; show one recipe's details        |
| `Layout/NavMenu.razor`  | Navigation link to the Add Recipe page                                |

**Flow:** `Home (list) → View details → /recipe/{id}` and `Home → Add Recipe → submit → NavigationManager → Home`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code
- Basic familiarity with C# and Razor syntax

---

## 🛠️ Steps

### Step 1 — Prepare the application

You'll create a Blazor WebAssembly application to manage recipes. Users will view a list of
recipes, add new recipes, and see detailed instructions for each recipe.

- Open **Visual Studio Code**.
- Open the terminal (`` Ctrl+` `` or **View > Terminal**) and create a new Blazor WebAssembly project:

```bash
dotnet new blazorwasm -o RecipeManagerApp
cd RecipeManagerApp
code .
dotnet run
```

- Create a folder called **`Data`** in the root directory of the project.

### Step 2 — Create the `Recipe` model and `RecipeService`

The model defines a recipe; the service is an in-memory store that the components share
through dependency injection.

Create **`Data/Recipe.cs`**:

```csharp
using System.ComponentModel.DataAnnotations;

namespace RecipeManagerApp.Data;

public class Recipe
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required, StringLength(500)]
    public string Description { get; set; } = "";
}
```

Create **`Data/RecipeService.cs`**:

```csharp
namespace RecipeManagerApp.Data;

public class RecipeService
{
    private readonly List<Recipe> _recipes = new()
    {
        new Recipe { Id = 1, Name = "Pasta Carbonara", Description = "Creamy sauce with bacon and parmesan." },
        new Recipe { Id = 2, Name = "Greek Salad", Description = "Tomatoes, cucumbers, olives, feta." },
        new Recipe { Id = 3, Name = "Banana Bread", Description = "Moist, sweet loaf with ripe bananas." }
    };

    public IReadOnlyList<Recipe> GetAll() => _recipes;

    public Recipe? GetById(int id) => _recipes.FirstOrDefault(r => r.Id == id);

    public void Add(Recipe recipe)
    {
        recipe.Id = (_recipes.LastOrDefault()?.Id ?? 0) + 1;
        _recipes.Add(recipe);
    }
}
```

Register the service as a **singleton** in `Program.cs` so all components share the same
list for the app's lifetime:

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RecipeManagerApp;
using RecipeManagerApp.Data;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<RecipeService>();

await builder.Build().RunAsync();
```

### Step 3 — Modify the Home component

The Home page displays the list of recipes and lets users view details or navigate to the
Add Recipe page. It binds a `List<Recipe>` to the UI and renders each recipe's name and
description with a link to its details.

Open **`Pages/Home.razor`** and replace its contents:

```razor
@page "/"
@using RecipeManagerApp.Data
@inject RecipeService RecipeService

<h1 class="mb-3">Recipes</h1>

<div class="mb-3">
    <a class="btn btn-primary" href="/addrecipe">Add Recipe</a>
</div>

@if (recipes.Count == 0)
{
    <p>No recipes yet. Click <strong>Add Recipe</strong> to create one.</p>
}
else
{
    <ul class="list-group">
        @foreach (var r in recipes)
        {
            <li class="list-group-item d-flex justify-content-between align-items-start">
                <div class="me-auto">
                    <div class="fw-bold">
                        <a href="@($"/recipe/{r.Id}")">@r.Name</a>
                    </div>
                    <small>@r.Description</small>
                </div>
                <a class="btn btn-sm btn-outline-secondary" href="@($"/recipe/{r.Id}")">View details</a>
            </li>
        }
    </ul>
}

@code {
    private List<Recipe> recipes = new();

    protected override void OnInitialized()
    {
        recipes = RecipeService.GetAll().ToList();
    }
}
```

### Step 4 — Create the Add Recipe page

The Add Recipe page lets users add a new recipe. It uses an `EditForm` with **two-way
binding** (`@bind-Value`) on the inputs and the `NavigationManager` service to return to the
Home page after a successful submit.

Create **`Pages/AddRecipe.razor`**:

```razor
@page "/addrecipe"
@using RecipeManagerApp.Data
@inject RecipeService RecipeService
@inject NavigationManager Nav

<h3>Add Recipe</h3>

<EditForm Model="@model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="mb-3">
        <label class="form-label">Name</label>
        <InputText class="form-control" @bind-Value="model.Name" />
    </div>
    <div class="mb-3">
        <label class="form-label">Description</label>
        <InputTextArea class="form-control" @bind-Value="model.Description" rows="4" />
    </div>

    <button type="submit" class="btn btn-primary">Add</button>
    <button type="button" class="btn btn-secondary ms-2" @onclick="Cancel">Cancel</button>
</EditForm>

@code {
    private Recipe model = new();

    private void HandleSubmit()
    {
        RecipeService.Add(model);
        Nav.NavigateTo("/");
    }

    private void Cancel()
    {
        Nav.NavigateTo("/");
    }
}
```

### Step 5 — Add dynamic routing for recipe details

Enable users to navigate to a dynamically routed Recipe Details page. The `{id:int}` route
constraint binds the URL segment to an `[Parameter]`, and the recipe is fetched in
`OnParametersSet` so it refreshes whenever the route value changes.

Create **`Pages/RecipeDetails.razor`**:

```razor
@page "/recipe/{id:int}"
@using RecipeManagerApp.Data
@inject RecipeService RecipeService
@inject NavigationManager Nav

<h3>Recipe Details</h3>

@if (recipe is null)
{
    <div class="alert alert-warning">Recipe not found.</div>
    <button class="btn btn-secondary" @onclick="GoBack">Back</button>
}
else
{
    <div class="card">
        <div class="card-body">
            <h4 class="card-title">@recipe.Name</h4>
            <p class="card-text">@recipe.Description</p>
        </div>
    </div>
    <div class="mt-3">
        <button class="btn btn-secondary" @onclick="GoBack">Back</button>
    </div>
}

@code {
    [Parameter] public int id { get; set; }

    private Recipe? recipe;

    protected override void OnParametersSet()
    {
        recipe = RecipeService.GetById(id);
    }

    private void GoBack() => Nav.NavigateTo("/");
}
```

### Step 6 — Add navigation links

Add a navigation link so users can switch to the Add Recipe page from the side menu.

Open **`Layout/NavMenu.razor`** and add an item inside the `<nav>` list:

```razor
<div class="nav-item px-3">
    <NavLink class="nav-link" href="addrecipe">
        <span class="bi bi-plus-square-fill-nav-menu" aria-hidden="true"></span> Add Recipe
    </NavLink>
</div>
```

For reference, **`Layout/MainLayout.razor`** keeps the standard shell that hosts every page
through `@Body`:

```razor
@inherits LayoutComponentBase

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>
    <main>
        <div class="top-row px-4">
            <a href="https://learn.microsoft.com/aspnet/core/" target="_blank">About</a>
        </div>
        <article class="content px-4">
            @Body
        </article>
    </main>
</div>
```

---

## ▶️ Expected result

Run the app with `dotnet run` and open it in the browser:

- The **Home** page lists the three seeded recipes, each with its name and description.
- Clicking a recipe name or **View details** opens `/recipe/{id}` showing that recipe; an
  unknown id shows "Recipe not found."
- **Add Recipe** opens the form; submitting a valid name and description adds the recipe and
  navigates back to Home, where the new entry now appears in the list.

---

## ☑️ Definition of done

- [ ] `RecipeManagerApp` Blazor WebAssembly project created and runs with `dotnet run`
- [ ] `Data/Recipe.cs` model and `Data/RecipeService.cs` store exist, and `RecipeService` is registered in `Program.cs`
- [ ] `Home.razor` binds the recipe list to the UI with links to details and the add page
- [ ] `AddRecipe.razor` uses an `EditForm` with `@bind-Value` and adds a recipe, then navigates home via `NavigationManager`
- [ ] `RecipeDetails.razor` uses the `@page "/recipe/{id:int}"` route and shows the matching recipe (or a not-found message)
- [ ] `NavMenu.razor` includes a link to `/addrecipe`

---

## 🔑 Key concepts

- **One-way vs. two-way binding** — the list is rendered with one-way interpolation
  (`@r.Name`), while form inputs use `@bind-Value` to keep the `Recipe` model and the UI in
  sync as the user types.
- **`EditForm` + DataAnnotations** — `[Required]` / `[StringLength]` on the model plus
  `DataAnnotationsValidator` give validated input for free; `OnValidSubmit` only fires when
  the form is valid.
- **Event handling** — `@onclick` and `OnValidSubmit` map UI events to C# methods, the core
  of Blazor's interactivity.
- **Route parameters & constraints** — `@page "/recipe/{id:int}"` binds a URL segment to an
  `[Parameter]`; the `:int` constraint rejects non-numeric ids before the component renders.
- **Programmatic navigation & shared state** — `NavigationManager.NavigateTo` moves between
  pages from code, and a singleton `RecipeService` lets every component read and write the
  same recipe list.
