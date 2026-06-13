# Designing Reusable Components

**Course 4 — Blazor for Front-End Development** · Module 2 · Lesson 2 · `You Try It!`

> Build a Blazor WebAssembly app named **`AdvancedBlazorComponents`** that combines three
> advanced features into one reusable component: **Dependency Injection** (a service that
> supplies data), **Cascading Parameters** (a theme color flowing down the tree), and
> **Component References** (a parent driving a child via `@ref`). The goal is a flexible,
> self-contained component you can drop in anywhere.

---

## 🎯 Objective

By the end of this activity, you will implement advanced Blazor component features —
**Dependency Injection**, **Cascading Parameters**, and **Component References** — to create
reusable and flexible components.

---

## 🗂️ What you will build

A Blazor WebAssembly project named **`AdvancedBlazorComponents`** with these pieces:

| File / Folder                     | Responsibility                                                            |
| --------------------------------- | ------------------------------------------------------------------------- |
| `Services/IDataService.cs`        | Contract for fetching data                                                |
| `Services/DataService.cs`         | Mock data source — returns a shuffled list of tag strings                 |
| `Components/ReusableComponent.razor` | The reusable component: injects the service, reads a cascading theme, holds a child via `@ref` |
| `Components/ChildComponent.razor` | Child counter the parent controls (`IncrementBy`, `Reset`, read `Count`)  |
| `Layout/MainLayout.razor`         | Wraps content in a `CascadingValue` named `ThemeColor`                    |
| `Pages/Index.razor`               | Hosts the reusable component at route `/`                                 |
| `Program.cs`                      | Registers `IDataService` in DI                                            |

**Flow:** `DataService → (DI) → ReusableComponent` · `MainLayout → (CascadingValue) → ThemeColor` · `ReusableComponent → (@ref) → ChildComponent`

---

## ✅ Prerequisites

- .NET SDK **6.0 or later** installed — check with `dotnet --version`
- Visual Studio Code
- A terminal (VS Code: **View > Terminal** or `Ctrl + ~`)

---

## 🛠️ Steps

### Step 1 — Prepare the application

Create a new Blazor WebAssembly app, open it, and confirm it runs.

```bash
dotnet new blazorwasm -o AdvancedBlazorComponents
cd AdvancedBlazorComponents
code .
dotnet run
```

You should see `Now listening on: http://localhost:5000` (your port number may differ). Stop
the app with `Ctrl + C` when you are ready to edit.

Then prepare the entry page:

- In the `Pages` folder, make sure there is an `Index.razor` with a routing directive so it is
  loadable at `/`.
- Remove the default content and host the reusable component there.

```razor
@* Pages/Index.razor *@
@page "/"
@using AdvancedBlazorComponents.Components

<h1 class="mb-4">Welcome to the Lab</h1>
<p>This page shows <strong>Dependency Injection</strong>, <strong>Cascading Parameters</strong>, and <strong>Parent→Child</strong> communication.</p>

<ReusableComponent />
```

### Step 2 — Implement Dependency Injection

Create a service that supplies data, register it, and inject it into the component.

- Create a **`Services`** folder.
- Add the service contract and a mock implementation. `GetData` returns a shuffled list of strings.

```csharp
// Services/IDataService.cs
namespace AdvancedBlazorComponents.Services;

public interface IDataService
{
    Task<List<string>> GetData();
}
```

```csharp
// Services/DataService.cs
namespace AdvancedBlazorComponents.Services;

public class DataService : IDataService
{
    private readonly Random _rng = new();

    public Task<List<string>> GetData()
    {
        var techs = new List<string>
        {
            "Blazor", "C#", "Dependency Injection", ".NET 9", "WebAssembly",
            "Reusable UI", "Razor Components", "CSS Animations",
            "Parent-Child Communication", "Bootstrap 5"
        };
        return Task.FromResult(techs.OrderBy(_ => _rng.Next()).ToList());
    }
}
```

Register the service in **`Program.cs`** so Blazor can inject it:

```csharp
// Program.cs
using AdvancedBlazorComponents;
using AdvancedBlazorComponents.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IDataService, DataService>();   // <-- register the service

await builder.Build().RunAsync();
```

Create the reusable component (a `Components` folder is used here) and **inject** the service.
The full component is shown in Step 4 once every feature is wired together; for now, inject and
render the data:

```razor
@inject IDataService DataService
@code {
    private List<string>? items;
    protected override async Task OnInitializedAsync() => items = await DataService.GetData();
}
```

Test it with `dotnet run` and browse to `http://localhost:5000/` to see the reusable component.

### Step 3 — Utilize Cascading Parameters

Share a `ThemeColor` from the layout down to the component without passing it through every parameter.

- Open **`Layout/MainLayout.razor`** and wrap the layout body in a `CascadingValue` named `ThemeColor`.

```razor
@* Layout/MainLayout.razor *@
@inherits LayoutComponentBase

<div class="page">
    <main>
        <CascadingValue Name="ThemeColor" Value="@_themeColor">
            <article class="content px-4">
                @Body
            </article>
        </CascadingValue>
    </main>
</div>

@code {
    private string _themeColor = "#6d28d9";
}
```

- In the component, receive it with `[CascadingParameter(Name = "ThemeColor")]` and render a
  `<p>` whose `color` is set to the cascaded value (see Step 4).

Stop and restart the application, then re-test `Index.razor`.

### Step 4 — Leverage Component References

Add a child component and let the parent drive it through a `@ref`.

- Create **`Components/ChildComponent.razor`** — a small counter the parent can control.

```razor
@* Components/ChildComponent.razor *@
@using Microsoft.AspNetCore.Components
@namespace AdvancedBlazorComponents.Components

<div class="p-3 border rounded" style="border-color:@(AccentColor ?? "#d0d7de"); max-width:420px; margin:auto;">
    <h5 style="color:@(AccentColor ?? "inherit")">🧒 Child Counter</h5>
    <p>Count: <strong>@Count</strong></p>
    <div class="d-flex gap-2">
        <button class="btn btn-success btn-animate flex-fill" @onclick="() => IncrementBy(1)">+1</button>
        <button class="btn btn-danger  btn-animate flex-fill" @onclick="Reset">Reset</button>
    </div>
</div>

@code {
    [Parameter] public string? AccentColor { get; set; }
    public int Count { get; private set; }

    public void IncrementBy(int n)
    {
        Count += n;
        _ = InvokeAsync(StateHasChanged);
    }

    public void Reset()
    {
        Count = 0;
        _ = InvokeAsync(StateHasChanged);
    }
}
```

- Include `ChildComponent` inside `ReusableComponent.razor`, capture it with `@ref="child"`, and
  call its public methods (`IncrementBy`, `Reset`) and read its `Count` from the parent.

Here is the complete **`ReusableComponent.razor`** bringing DI, the cascading theme, and the child reference together:

```razor
@* Components/ReusableComponent.razor *@
@using AdvancedBlazorComponents.Services
@inject IDataService DataService
@using AdvancedBlazorComponents.Components

<div class="card p-3 my-3 shadow-sm border rounded" style="max-width:900px;">
    <h3>🎨 Tag Cloud (DI + Theme)</h3>
    <p style="color:@(ThemeColor ?? "inherit")">
        Theme color preview (current: @(ThemeColor ?? "default"))
    </p>

    @if (items is null)
    {
        <p>Loading...</p>
    }
    else
    {
        <div class="tag-cloud mb-3">
            @foreach (var tag in items)
            {
                var h = HueFrom(tag);
                <span class="badge me-1" style="background-color:hsl(@(h), 70%, 80%)">@tag</span>
            }
        </div>
    }

    <button class="btn btn-outline-secondary mb-4" @onclick="Reload">🔄 Reload Tags</button>
    <hr />

    <h4>Parent → Child (via @@ref)</h4>
    <ChildComponent @ref="child" AccentColor="@(ThemeColor)" />
    <div class="d-flex gap-2 mt-2" style="max-width:420px; margin:auto;">
        <button class="btn btn-primary flex-fill" @onclick="() => child?.IncrementBy(5)">Child +5</button>
        <button class="btn btn-warning flex-fill" @onclick="() => child?.Reset()">Reset Child</button>
        <button class="btn btn-outline-dark flex-fill" @onclick="ReadChildCount">Read Count</button>
    </div>

    @if (lastRead.HasValue)
    {
        <p class="text-center mt-2">Parent read: Child.Count = <strong>@lastRead</strong></p>
    }
</div>

@code {
    [CascadingParameter(Name = "ThemeColor")] public string? ThemeColor { get; set; }
    private List<string>? items;
    private ChildComponent? child;
    private int? lastRead;

    protected override async Task OnInitializedAsync()
        => items = await DataService.GetData();

    private async Task Reload() =>
        items = await DataService.GetData();

    private void ReadChildCount() =>
        lastRead = child?.Count;

    private int HueFrom(string s)
    {
        unchecked
        {
            int hash = 0;
            foreach (var c in s) hash = (hash * 31) + c;
            return Math.Abs(hash % 360);
        }
    }
}
```

Test `Index.razor` once more with the changes in place.

> *Note:* the component places `ReusableComponent.razor` / `ChildComponent.razor` under a
> `Components` folder (matching the `@using AdvancedBlazorComponents.Components` namespace). The
> activity's wording mentions the `Pages` folder — either works, as long as the folder and the
> `@using`/`@namespace` directives agree.

---

## ▶️ Expected result

Browsing to `http://localhost:5000/` shows the reusable **Tag Cloud** card:

- a shuffled set of colored technology **badges** supplied by the injected `DataService`, with a
  **🔄 Reload Tags** button that re-shuffles them;
- a theme-colored preview line driven by the cascaded **`ThemeColor`**;
- a **🧒 Child Counter** whose buttons (**Child +5**, **Reset Child**, **Read Count**) are all
  invoked by the parent through the `@ref`, with the last read count echoed back in the parent.

---

## ☑️ Definition of done

- [ ] `AdvancedBlazorComponents` Blazor WebAssembly project created and running on `http://localhost:5000`
- [ ] `IDataService` + `DataService` implemented and registered with `AddScoped<IDataService, DataService>()` in `Program.cs`
- [ ] `ReusableComponent.razor` injects `IDataService` and renders the tag cloud from `GetData()`
- [ ] `MainLayout.razor` provides a `CascadingValue` named `ThemeColor`, consumed via `[CascadingParameter]`
- [ ] `ChildComponent.razor` is captured with `@ref` and driven from the parent (`IncrementBy`, `Reset`, read `Count`)
- [ ] `Index.razor` routes at `/` and hosts `ReusableComponent`

---

## 🔑 Key concepts

- **Dependency Injection** — components depend on the `IDataService` *interface*, not a concrete
  class; registering it in `Program.cs` lets Blazor supply it (and lets you swap in a real data
  source or a test double without touching the UI).
- **Cascading Parameters** — a `CascadingValue` in `MainLayout` flows `ThemeColor` down to any
  descendant that declares `[CascadingParameter(Name = "ThemeColor")]`, avoiding "prop drilling"
  through every intermediate component.
- **Component References (`@ref`)** — capturing a child instance lets the parent call its public
  methods (`IncrementBy`, `Reset`) and read its state (`Count`) directly — imperative control on
  top of declarative rendering.
- **Reusability through composition** — combining an injected service, a cascaded value, and a
  referenced child yields a flexible, drop-in component that owns its data and behavior.
- **`StateHasChanged` / `InvokeAsync`** — when state changes outside the normal event flow, the
  child re-renders by queuing `StateHasChanged` via `InvokeAsync` to stay on the UI thread.
