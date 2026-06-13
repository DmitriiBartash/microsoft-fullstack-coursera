# Implementing State Management

**Course 4 — Blazor for Front-End Development** · Module 3 · Lesson 2 · `You Try It!`

> Build a Blazor WebAssembly app — **`FeedbackApp`** — that collects user feedback through
> a validated `<EditForm>`, stores submissions in an injected **state service**, and renders
> them in a list. The lab ties together three Blazor pillars: **two-way data binding**,
> **DataAnnotations validation**, and **in-memory state management via DI**.

---

## 🎯 Objective

Create a Blazor WebAssembly application that demonstrates **state management** and includes
a **form with validation** to collect user feedback. By the end you will have a working form
that validates input, persists each entry in a singleton-style service, and displays all
collected feedback on a separate page.

---

## 🗂️ What you will build

A Blazor WebAssembly project named **`FeedbackApp`** made of these pieces:

| File                      | Responsibility                                                        |
| ------------------------- | --------------------------------------------------------------------- |
| `Models/FeedbackItem.cs`  | Data model with validation attributes (`Name`, `Email`, `Comment`)    |
| `Services/IFeedbackService.cs` | Abstraction for adding and retrieving feedback                   |
| `Services/FeedbackService.cs`  | In-memory state store backing the feedback list                  |
| `Pages/FeedbackForm.razor`     | Validated `<EditForm>` that captures and submits feedback        |
| `Pages/FeedbackList.razor`     | Table view that reads feedback from the service                  |
| `Program.cs`              | Registers `FeedbackService` for dependency injection                  |

**Flow:** `FeedbackForm  →  EditForm (validate)  →  FeedbackService.Add()  →  state  →  FeedbackList.GetAll()`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- The Blazor WebAssembly templates (ship with the .NET SDK)
- Visual Studio Code

---

## 🛠️ Steps

### Step 1 — Prepare for the application

Scaffold a new Blazor WebAssembly project to manage user feedback, then open it.

```bash
dotnet new blazorwasm -o FeedbackApp
cd FeedbackApp
```

- Open the `FeedbackApp` folder in Visual Studio Code.

### Step 2 — Set up the data model

Define a model to store feedback details, including validation attributes.

- Create a folder named **`Models`** in the project root.
- Inside `Models`, create a file named **`FeedbackItem.cs`**.
- Define a `FeedbackItem` class with three properties:
  - `Name` — a **required** string for the user's name.
  - `Email` — a string validated as an **email** address.
  - `Comment` — a string limited to a **maximum length** of 500 characters.

```csharp
using System.ComponentModel.DataAnnotations;

namespace FeedbackApp.Models
{
    public class FeedbackItem
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string Comment { get; set; } = string.Empty;
    }
}
```

### Step 3 — Implement state management

Store and manage submitted feedback in the application state behind a service abstraction.

- Create a new folder named **`Services`**.
- Add **`IFeedbackService.cs`** — the contract for adding and retrieving feedback.
- Add **`FeedbackService.cs`** — an in-memory implementation that holds the list.

```csharp
// Services/IFeedbackService.cs
using FeedbackApp.Models;

namespace FeedbackApp.Services;

public interface IFeedbackService
{
    void Add(FeedbackItem item);
    IReadOnlyList<FeedbackItem> GetAll();
}
```

```csharp
// Services/FeedbackService.cs
using System.Collections.ObjectModel;
using FeedbackApp.Models;

namespace FeedbackApp.Services;

public class FeedbackService : IFeedbackService
{
    private readonly ObservableCollection<FeedbackItem> _items = new();

    public void Add(FeedbackItem item)
    {
        if (item is null) return;

        _items.Add(new FeedbackItem
        {
            Name = item.Name,
            Email = item.Email,
            Comment = item.Comment
        });
    }

    public IReadOnlyList<FeedbackItem> GetAll()
        => new ReadOnlyCollection<FeedbackItem>(_items);
}
```

> The service stores a **copy** of each submitted item, so later edits to the bound form model
> cannot mutate already-stored feedback.

### Step 4 — Register the service for dependency injection

Wire the service into the DI container so components can inject it.

- Open **`Program.cs`** and register `FeedbackService` against its interface:

```csharp
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
```

> In Blazor WebAssembly the app is single-user, so a `Scoped` registration behaves like a
> singleton for the lifetime of the loaded app — keeping the feedback list alive as you
> navigate between pages.

### Step 5 — Build the feedback form & handle submission

Create a form to capture user feedback, validate it, and persist it on submit.

- Open the **`Pages`** folder and create **`FeedbackForm.razor`**.
- Use the Blazor form components `<EditForm>`, `<InputText>`, `<InputTextArea>`, and
  `<ValidationSummary>`, binding inputs to the `FeedbackItem` model.
- Add `<DataAnnotationsValidator>` to enable validation from the model's attributes.
- In the `OnValidSubmit` handler, inject `IFeedbackService`, add the feedback, show a
  confirmation message, and reset the form.

```razor
@page "/feedback"
@using FeedbackApp.Models
@using FeedbackApp.Services
@inject IFeedbackService FeedbackService

<h3>Submit Feedback</h3>

<EditForm Model="@model" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="mb-3">
        <label class="form-label">Name</label>
        <InputText @bind-Value="model.Name" class="form-control" />
        <ValidationMessage For="@(() => model.Name)" />
    </div>

    <div class="mb-3">
        <label class="form-label">Email</label>
        <InputText @bind-Value="model.Email" class="form-control" />
        <ValidationMessage For="@(() => model.Email)" />
    </div>

    <div class="mb-3">
        <label class="form-label">Comment</label>
        <InputTextArea @bind-Value="model.Comment" class="form-control" rows="5" />
        <ValidationMessage For="@(() => model.Comment)" />
    </div>

    <button type="submit" class="btn btn-primary">Submit</button>
</EditForm>

@if (submitted)
{
    <div class="alert alert-success mt-3">
        Thanks for your feedback, @lastSubmittedName!
    </div>
}

@code {
    private FeedbackItem model = new();
    private bool submitted;
    private string lastSubmittedName = string.Empty;

    private void HandleValidSubmit()
    {
        FeedbackService.Add(model);
        lastSubmittedName = model.Name;
        submitted = true;
        model = new();
    }
}
```

### Step 6 — Display submitted feedback

Create a component that lists every stored feedback entry.

- Create **`Pages/FeedbackList.razor`**.
- Retrieve the data from `FeedbackService` in `OnInitialized`.
- Render the entries in a Blazor `<table>`, with a friendly empty state.

```razor
@page "/feedback/list"
@using FeedbackApp.Models
@using FeedbackApp.Services
@inject IFeedbackService FeedbackService

<h3>Feedback List</h3>

<div class="mb-3">
    <NavLink class="btn btn-primary" href="feedback">+ Add Feedback</NavLink>
</div>

@if (items.Count == 0)
{
    <p>No feedback yet.</p>
    <NavLink class="btn btn-outline-primary" href="feedback">Create first feedback</NavLink>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Comment</th>
            </tr>
        </thead>
        <tbody>
        @foreach (var f in items)
        {
            <tr>
                <td>@f.Name</td>
                <td>@f.Email</td>
                <td>@f.Comment</td>
            </tr>
        }
        </tbody>
    </table>
}

@code {
    private IReadOnlyList<FeedbackItem> items = Array.Empty<FeedbackItem>();

    protected override void OnInitialized()
    {
        items = FeedbackService.GetAll();
    }
}
```

### Step 7 — Run the application

```bash
dotnet run
```

- Browse to `/feedback`, submit the form, then navigate to `/feedback/list` to see the entry.

---

## ▶️ Expected result

Navigating to `/feedback` shows the validated form. Submitting **valid** data displays
*"Thanks for your feedback, &lt;name&gt;!"* and clears the inputs; submitting **invalid** data
(blank name, malformed email, or an over-length comment) shows inline validation messages and
blocks the submit. On `/feedback/list` every stored submission appears in the table, persisting
as you move between the two pages.

---

## ☑️ Definition of done

- [ ] `FeedbackApp` Blazor WebAssembly project created and opened
- [ ] `Models/FeedbackItem.cs` defines `Name` (required), `Email` (email), `Comment` (max 500)
- [ ] `IFeedbackService` and `FeedbackService` store feedback in memory
- [ ] `FeedbackService` registered in `Program.cs` via dependency injection
- [ ] `FeedbackForm.razor` validates input and adds feedback on `OnValidSubmit`
- [ ] `FeedbackList.razor` reads from the service and renders the table
- [ ] `dotnet run` serves the app; valid submissions appear in the list, invalid ones are rejected

---

## 🔑 Key concepts

- **Two-way data binding** — `@bind-Value` on `<InputText>` / `<InputTextArea>` keeps the UI and
  the `FeedbackItem` model in sync without manual event wiring.
- **Declarative validation** — DataAnnotations (`[Required]`, `[EmailAddress]`, `[MaxLength]`)
  plus `<DataAnnotationsValidator>` drive `OnValidSubmit` and the `<ValidationMessage>` output.
- **State via DI** — a service registered in the container holds application state and is shared
  across components, so navigating away and back does not lose the collected feedback.
- **Defensive storage** — `FeedbackService.Add` copies the item and `GetAll` returns a read-only
  view, preventing callers from mutating the internal collection.
- **Routing between components** — `@page` directives and `<NavLink>` connect the form
  (`/feedback`) and the list (`/feedback/list`) into a small navigable flow.
