# Implementing Client-Side State Management in Blazor

**Course 7 — Full-Stack Integration** · Module 3 · Lesson 1 · `You Try It!`

> Build a **Blazor WebAssembly** app that persists UI state in the browser. You'll save a
> theme preference to **local storage**, keep a shopping cart in **session storage**, and add
> a button that **clears both** — the three everyday tools of client-side state management.

---

## 🎯 Objective

By the end of this activity, you will be able to implement client-side state management
techniques in a Blazor WebAssembly application — using **local storage** for data that should
survive a browser restart, **session storage** for data scoped to the current tab, and
**clearing stored data** programmatically.

---

## 🗂️ What you'll build

A Blazor WebAssembly project named **`ClientStateApp`** with these pieces:

| Piece                         | Responsibility                                               |
| ----------------------------- | ----------------------------------------------------------- |
| `Blazored.LocalStorage`       | Persist the user's **theme** across browser sessions        |
| `Blazored.SessionStorage`     | Hold the **shopping cart** for the current tab only          |
| `Pages/Home.razor`            | Theme selector + a **Clear All Storage** button             |
| `Pages/Cart.razor`            | Add/list cart items, persisted to session storage           |
| `Program.cs`                  | Register both storage services with DI                      |

**Flow:** `Home (theme) → localStorage` · `Cart (items) → sessionStorage` · `Clear → wipes both`

---

## ✅ Prerequisites

- **.NET 6 SDK or higher** — check with `dotnet --version`
- Visual Studio Code (with the C# / C# Dev Kit extension)
- NuGet packages: `Blazored.LocalStorage`, `Blazored.SessionStorage`

---

## 🛠️ Steps

### Step 1 — Prepare the application

Create a new Blazor WebAssembly project, move into it, and open it in VS Code.

```bash
dotnet new blazorwasm -o ClientStateApp
cd ClientStateApp
code .
```

- Confirm the app runs with `dotnet run`, then stop it (`Ctrl+C`) before continuing.

### Step 2 — Implement local storage (theme preference)

Persist the user's theme so it survives a full browser restart.

Install the package:

```bash
dotnet add package Blazored.LocalStorage
```

Register the service in **`Program.cs`**:

```csharp
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ClientStateApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Client-side storage services
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
```

Edit **`Pages/Home.razor`** to add a theme selector that reads on load and writes on change:

```razor
@page "/"
@using Blazored.LocalStorage
@inject ILocalStorageService LocalStorage

<PageTitle>Home</PageTitle>

<h1>Theme preference</h1>

<p>Current theme: <strong>@_theme</strong></p>

<select value="@_theme" @onchange="OnThemeChanged">
    <option value="light">Light</option>
    <option value="dark">Dark</option>
</select>

@code {
    private const string ThemeKey = "theme";
    private string _theme = "light";

    protected override async Task OnInitializedAsync()
    {
        // Retrieve the saved theme on initialization
        var saved = await LocalStorage.GetItemAsync<string>(ThemeKey);
        if (!string.IsNullOrWhiteSpace(saved))
        {
            _theme = saved;
        }
    }

    private async Task OnThemeChanged(ChangeEventArgs e)
    {
        _theme = e.Value?.ToString() ?? "light";
        // Save the theme preference to local storage
        await LocalStorage.SetItemAsync(ThemeKey, _theme);
    }
}
```

### Step 3 — Use session storage (shopping cart)

Store cart items in session storage so they live only as long as the browser tab.

Install the package:

```bash
dotnet add package Blazored.SessionStorage
```

Register it in **`Program.cs`** alongside local storage:

```csharp
// Client-side storage services
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
```

Create a new component **`Pages/Cart.razor`**:

```razor
@page "/cart"
@using Blazored.SessionStorage
@inject ISessionStorageService SessionStorage

<PageTitle>Cart</PageTitle>

<h1>Shopping cart</h1>

<input @bind="_newItem" placeholder="Add an item" />
<button @onclick="AddItem">Add</button>

<ul>
    @foreach (var item in _items)
    {
        <li>@item</li>
    }
</ul>

@code {
    private const string CartKey = "cart";
    private List<string> _items = new();
    private string _newItem = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        // Retrieve the cart contents on initialization
        var saved = await SessionStorage.GetItemAsync<List<string>>(CartKey);
        if (saved is not null)
        {
            _items = saved;
        }
    }

    private async Task AddItem()
    {
        if (string.IsNullOrWhiteSpace(_newItem))
        {
            return;
        }

        _items.Add(_newItem.Trim());
        _newItem = string.Empty;

        // Save the cart contents to session storage
        await SessionStorage.SetItemAsync(CartKey, _items);
    }
}
```

Add a link to the cart in **`Shared/NavMenu.razor`** (inside the existing `nav` list):

```razor
<div class="nav-item px-3">
    <NavLink class="nav-link" href="cart">
        <span class="oi oi-cart" aria-hidden="true"></span> Cart
    </NavLink>
</div>
```

### Step 4 — Clear storage

Add a button to **`Home.razor`** that wipes both local and session storage.

Inject the session storage service and add the button + handler to **`Pages/Home.razor`**:

```razor
@inject ISessionStorageService SessionStorage
```

```razor
<button class="btn btn-danger" @onclick="ClearAllStorage">Clear All Storage</button>

@if (_cleared)
{
    <p><em>All stored data cleared.</em></p>
}
```

```csharp
private bool _cleared;

private async Task ClearAllStorage()
{
    // Wipe every key from both stores
    await LocalStorage.ClearAsync();
    await SessionStorage.ClearAsync();

    _theme = "light";
    _cleared = true;
}
```

Run the app and verify the behavior:

```bash
dotnet run
```

---

## ▶️ Expected result

- Pick a theme on **Home**, refresh (or fully restart the browser) — the choice **persists** via local storage.
- Add items on **Cart**, refresh the tab — items **persist**; open a *new* tab and the cart is **empty** (session-scoped).
- Click **Clear All Storage**, then refresh — both the saved theme and the cart are **gone**.

---

## ☑️ Definition of done

- [ ] `ClientStateApp` Blazor WebAssembly project created and runs
- [ ] `Blazored.LocalStorage` and `Blazored.SessionStorage` packages added and registered in `Program.cs`
- [ ] `Home.razor` saves and restores the theme via **local storage**
- [ ] `Cart.razor` saves and restores cart items via **session storage**
- [ ] **Clear All Storage** button empties both stores
- [ ] Refresh/restart behavior matches the expected result for each store

---

## 🔑 Key concepts

- **Local vs. session storage** — local storage persists across browser restarts and tabs; session
  storage is cleared when the tab closes and is **not** shared between tabs. Choose by lifetime.
- **DI registration is required** — `AddBlazoredLocalStorage()` / `AddBlazoredSessionStorage()` in
  `Program.cs` make `ILocalStorageService` / `ISessionStorageService` injectable into components.
- **Read on init, write on change** — load persisted state in `OnInitializedAsync` and save in the
  event handler so the UI and storage stay in sync.
- **Serialization is automatic** — Blazored serializes objects (like `List<string>`) to JSON, so you
  can store and retrieve typed data with `SetItemAsync` / `GetItemAsync<T>`.
- **Client-side storage isn't secure** — it's plain text in the browser; never keep secrets or
  sensitive data there, and call `ClearAsync` to remove state on sign-out or reset.
