# Final project

**Course 4 — Blazor for Front-End Development** · Module 5 · `Activity`

> Build **EventEase**, an event-management web app, in three Copilot-assisted activities:
> **generate** the foundational Event Card component and routing, **debug & optimize** it,
> then **expand** it with state management and form validation. Microsoft Copilot is your
> pair-programmer throughout — you drive the design, Copilot accelerates the code.

---

## 🎯 Objective

Use Microsoft Copilot to build a complete Blazor front-end for the EventEase app — starting
from a reusable **Event Card** component with two-way data binding and page routing, then
hardening it with validation and performance fixes, and finally extending it with state
management and registration forms ready for deployment.

---

## 🗂️ What you will build

The project runs across three activities that build on the same codebase:

| Activity | Focus | You produce |
| -------- | ----- | ----------- |
| **1 — Generate** | Foundational components | `EventCard` component with fields + two-way binding, and routing between event list / details / registration |
| **2 — Debug & optimize** | Reliability and performance | Input validation, graceful handling of invalid routes, faster rendering of large event lists |
| **3 — Expand** | Advanced features | State management, form validation, and a polished, user-friendly interface |

**Flow:** `Generate (EventCard + routing)  →  Debug & optimize  →  Expand (state + forms + UI)`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio (or VS Code) sandbox with **GitHub Copilot enabled**
- Basic familiarity with Blazor components and Razor syntax
- The EventEase scenario: a fictional company managing corporate and social events whose users need to **browse events** (name, date, location) and **navigate** between event details and registration

---

## 🛠️ Steps

### Step 1 — Scaffold the EventEase Blazor app

Create a Blazor Web App and run it once to confirm the toolchain works.

```bash
dotnet new blazor -n EventEase
cd EventEase
dotnet run
```

- Open the project in your sandbox and confirm **Copilot is enabled** (you should see inline suggestions while typing).

### Step 2 — Define an `Event` model (Activity 1)

Give Copilot a concrete shape to bind against. Add `Models/Event.cs`:

```csharp
namespace EventEase.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
}
```

### Step 3 — Generate the `EventCard` component with Copilot (Activity 1)

Create `Components/EventCard.razor`. Use Copilot to draft the markup, then refine it to expose
**event name, date, and location** with **two-way data binding** over a bound `Event`.

```razor
@namespace EventEase.Components
@using EventEase.Models

<div class="event-card">
    <label>Name:    <input @bind="EventItem.Name" /></label>
    <label>Date:    <input type="date" @bind="EventItem.Date" /></label>
    <label>Location:<input @bind="EventItem.Location" /></label>

    <h3>@EventItem.Name</h3>
    <p>@EventItem.Date.ToShortDateString() — @EventItem.Location</p>
</div>

@code {
    [Parameter] public Event EventItem { get; set; } = new();
}
```

- The `@bind` directive wires each input to the model and back, so edits update the display live.
- Use **mock data** for now; a real data source comes later.

### Step 4 — Set up routing between pages (Activity 1)

Create routable pages for the event **list**, **details**, and **registration**, and link them.
Let Copilot generate the `@page` directives and `NavLink` paths, then verify they follow Blazor
routing conventions.

```razor
@* Pages/Events.razor *@
@page "/events"
@using EventEase.Models
@using EventEase.Components

<h1>Events</h1>
@foreach (var ev in _events)
{
    <EventCard EventItem="ev" />
    <NavLink href="@($"/events/{ev.Id}")">Details</NavLink>
    <NavLink href="@($"/register/{ev.Id}")">Register</NavLink>
}

@code {
    private List<Event> _events = new()
    {
        new() { Id = 1, Name = "Tech Summit", Date = DateTime.Today.AddDays(7),  Location = "Seattle" },
        new() { Id = 2, Name = "Design Expo", Date = DateTime.Today.AddDays(14), Location = "Austin" },
    };
}
```

```razor
@* Pages/EventDetails.razor — route parameter *@
@page "/events/{Id:int}"

<h1>Event @Id</h1>

@code {
    [Parameter] public int Id { get; set; }
}
```

Confirm `<Router>` in `Routes.razor` (or `App.razor`) renders these pages and that navigation works.

### Step 5 — Debug with Copilot (Activity 2)

Initial testing of Activity 1 revealed three issues. Ask Copilot to analyze the code, then apply
fixes for each.

| Bug | Fix |
| --- | --- |
| Data binding fails on **invalid input** | Add validation so only valid data is processed in the Event Card |
| Routing **errors on non-existent pages** | Handle invalid paths gracefully (catch-all route + friendly message) |
| Event list is **slow on large datasets** | Improve how event data is rendered |

Add validation on the model and a catch-all route:

```csharp
// Models/Event.cs — data annotations drive built-in validation
using System.ComponentModel.DataAnnotations;

public class Event
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;
}
```

```razor
@* Pages/NotFound.razor — graceful handling of invalid paths *@
@page "/{*slug}"

<h1>Page not found</h1>
<p>That event or page doesn't exist. <NavLink href="/events">Back to events</NavLink>.</p>

@code {
    [Parameter] public string? Slug { get; set; }
}
```

### Step 6 — Optimize and validate fixes (Activity 2)

For large event lists, render only the visible rows with Blazor's built-in **`Virtualize`**
component instead of a plain `@foreach`.

```razor
<Virtualize Items="_events" Context="ev">
    <EventCard EventItem="ev" />
</Virtualize>
```

Then test the updated code:

- Verify data binding works for all edge cases (invalid or empty data is rejected).
- Test routing to confirm invalid paths land on the friendly **NotFound** page.
- Measure performance improvements for larger event datasets.

### Step 7 — Expand with state management and forms (Activity 3)

Bring everything together. Add a shared **state container** so registration data persists across
pages, and wrap the registration page in an `EditForm` with validation.

```csharp
// Services/EventState.cs — simple state container, registered in DI
namespace EventEase.Services;

public class EventState
{
    public List<int> RegisteredEventIds { get; } = new();
    public event Action? OnChange;

    public void Register(int eventId)
    {
        if (!RegisteredEventIds.Contains(eventId))
        {
            RegisteredEventIds.Add(eventId);
            OnChange?.Invoke();
        }
    }
}
```

```csharp
// Program.cs — register the state container
builder.Services.AddScoped<EventEase.Services.EventState>();
```

```razor
@* Pages/Register.razor — validated registration form *@
@page "/register/{Id:int}"
@inject EventEase.Services.EventState State

<EditForm Model="_registration" OnValidSubmit="Submit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <InputText @bind-Value="_registration.AttendeeName" placeholder="Your name" />
    <InputText @bind-Value="_registration.Email" placeholder="Email" />
    <button type="submit">Register</button>
</EditForm>

@code {
    [Parameter] public int Id { get; set; }
    private Registration _registration = new();

    private void Submit() => State.Register(Id);
}
```

Run the app and walk the full flow end-to-end:

```bash
dotnet run
```

---

## ▶️ Expected result

A working EventEase Blazor app: a reusable **Event Card** with two-way binding, **routing**
between the event list, details, and registration pages, **validation** that rejects bad input,
a friendly page for invalid URLs, **virtualized** rendering for large lists, and a **registration
form** whose state persists via the shared state container — a foundational app ready for
deployment.

---

## ☑️ Definition of done

- [ ] `EventEase` Blazor app scaffolds and runs with `dotnet run`
- [ ] `EventCard` component shows **name, date, location** with two-way `@bind`
- [ ] Routing links the **event list, details, and registration** pages
- [ ] Input validation rejects invalid/empty event data
- [ ] Invalid paths land on a graceful **NotFound** page
- [ ] Large event lists render efficiently (e.g. via `Virtualize`)
- [ ] Registration uses an `EditForm` with validation and shared `EventState`

---

## 🔑 Key concepts

- **Copilot as an accelerator, not the architect** — you decide the model, the routes, and the
  binding strategy; Copilot drafts and refines the code so you reach a working result faster.
- **Two-way data binding** — `@bind` keeps the UI and the model in sync in both directions, the
  backbone of an interactive Event Card.
- **Routing with parameters** — `@page "/events/{Id:int}"` and a catch-all `NotFound` route make
  navigation predictable and resilient to bad URLs.
- **Validate at the model** — data annotations plus `DataAnnotationsValidator` in an `EditForm`
  keep invalid data out without scattering checks through the UI.
- **Render large lists with `Virtualize`** — drawing only visible rows is the idiomatic Blazor fix
  for slow lists, far cheaper than a full `@foreach`.
- **Shared state via DI** — a scoped state container lets registration data outlive a single page
  and flow across the app.
