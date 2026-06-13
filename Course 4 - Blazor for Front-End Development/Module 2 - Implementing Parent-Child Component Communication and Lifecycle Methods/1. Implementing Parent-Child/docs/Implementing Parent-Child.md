# Implementing Parent-Child Component Communication

**Course 4 — Blazor for Front-End Development** · Module 2 · Lesson 1 · `You Try It!`

> Build a Blazor Server app where a **parent** component owns a list of tasks and renders a
> **child** component for each one. The parent passes data **down** with `[Parameter]`, and
> the child reports back **up** with an `EventCallback<string>` — completing a clean
> two-way communication loop.

---

## 🎯 Objective

By the end of this lab you will be able to implement **parent-to-child** and
**child-to-parent** communication in Blazor components using `[Parameter]` properties and
`EventCallback`, wiring them together so a child action updates parent state and re-renders
the UI.

---

## 🗂️ What you'll build

A Blazor Server project named **`TaskManagerApp`** with two Razor components:

| Component                  | Role   | Responsibility                                                        |
| -------------------------- | ------ | -------------------------------------------------------------------- |
| `ParentTaskManager.razor`  | Parent | Holds the task list, renders one child per task, handles completions |
| `ChildTaskDisplay.razor`   | Child  | Shows a single task and raises an event when it's marked complete    |

**Flow:** `ParentTaskManager` → `[Parameter] TaskName` → `ChildTaskDisplay` → button click → `EventCallback OnTaskCompleted` → `ParentTaskManager.HandleTaskCompleted`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code with the **C# Dev Kit** extension
- Basic familiarity with Razor components and the `@code` block

---

## 🛠️ Steps

### Step 1 — Prepare the application

Scaffold the Blazor Server app and open it in VS Code. This creates the project structure you'll add components to.

```bash
dotnet new blazorserver -n TaskManagerApp
cd TaskManagerApp
```

- Open the **`TaskManagerApp`** folder in Visual Studio Code.
- Under `Pages/` (or `Components/`), create two Razor components:
  - `ParentTaskManager.razor`
  - `ChildTaskDisplay.razor`

### Step 2 — Implement parent-to-child communication

The parent owns the task list and passes each task **name** down to a child using the `[Parameter]` attribute. A `foreach` loop renders one `ChildTaskDisplay` per task.

`ParentTaskManager.razor`

```razor
@page "/tasks"

<h3 class="mb-3">Task Manager (Parent)</h3>

@if (tasks.All(t => t.Completed))
{
    <div class="alert alert-success mb-3">🎉 All tasks are completed!</div>
}

<ul class="list-group">
    @foreach (var t in tasks)
    {
        <li class="list-group-item d-flex justify-content-between align-items-center">
            <span class="@(t.Completed ? "text-decoration-line-through text-muted" : null)">
                @t.Name
            </span>
            <ChildTaskDisplay
                TaskName="@t.Name"
                Completed="@t.Completed"
                OnTaskCompleted="HandleTaskCompleted" />
        </li>
    }
</ul>

<p class="mt-2 text-muted">Completed: @tasks.Count(x => x.Completed) / @tasks.Count</p>
```

### Step 3 — Implement child-to-parent communication

The child displays one task and exposes an `EventCallback<string>`. Clicking its button invokes that callback, notifying the parent which task was completed.

`ChildTaskDisplay.razor`

```razor
<p class="mb-1">Task: <strong>@TaskName</strong></p>

<button type="button"
        class="btn btn-sm @(Completed ? "btn-secondary" : "btn-success")"
        @onclick="MarkComplete"
        disabled="@(Completed)">
    @(Completed ? "✅ Completed" : "✔ Mark Complete")
</button>

@code {
    [Parameter] public string TaskName { get; set; } = string.Empty;
    [Parameter] public bool Completed { get; set; }
    [Parameter] public EventCallback<string> OnTaskCompleted { get; set; }

    private Task MarkComplete() => OnTaskCompleted.InvokeAsync(TaskName);
}
```

### Step 4 — Connect both directions

Back in the parent, handle the child's `OnTaskCompleted` event, update the matching task, and re-render. When every task is complete, the success banner from Step 2 appears automatically.

Add this `@code` block to **`ParentTaskManager.razor`**:

```razor
@code {
    private readonly List<TaskItem> tasks =
    [
        new("Complete Blazor lab"),
        new("Review EventCallback example"),
        new("Push code to GitHub"),
    ];

    private void HandleTaskCompleted(string taskName)
    {
        var item = tasks.FirstOrDefault(x => x.Name == taskName);
        if (item is not null)
        {
            item.Completed = true;
            StateHasChanged();
            Console.WriteLine($"Completed: {taskName}");
        }
    }

    private sealed record TaskItem(string Name)
    {
        public bool Completed { get; set; }
    }
}
```

### Step 5 — Test and run the application

Start the app and exercise the two-way flow end to end.

```bash
dotnet run
```

- Navigate to `/tasks` in the browser.
- Click **Mark Complete** on a task — the child raises the event, the parent strikes the
  task through, increments the counter, and disables the button.
- Complete every task to confirm the **🎉 All tasks are completed!** banner appears.

---

## ▶️ Expected result

The page lists three tasks, each with its own **Mark Complete** button. Clicking a button
updates the parent: the task name is struck through, the *Completed: n / 3* counter
advances, and the button switches to **✅ Completed**. Once all three are done, the green
**All tasks are completed!** alert renders at the top.

---

## ☑️ Definition of done

- [ ] `TaskManagerApp` Blazor Server project created and opened in VS Code
- [ ] `ParentTaskManager.razor` holds the task list and renders a `ChildTaskDisplay` per task
- [ ] Parent passes `TaskName` and `Completed` **down** via `[Parameter]`
- [ ] `ChildTaskDisplay.razor` raises `OnTaskCompleted` **up** through an `EventCallback<string>`
- [ ] Parent's `HandleTaskCompleted` marks the task complete and calls `StateHasChanged()`
- [ ] `dotnet run` shows tasks updating and the "all completed" banner when finished

---

## 🔑 Key concepts

- **Parameters flow down** — a parent shares data with a child by binding values to the
  child's `[Parameter]` properties (`TaskName="@t.Name"`), keeping ownership of state in the parent.
- **EventCallbacks flow up** — a child can't mutate parent state directly; it raises an
  `EventCallback<T>` (`OnTaskCompleted.InvokeAsync(TaskName)`) so the parent decides how to respond.
- **The parent is the source of truth** — `ParentTaskManager` owns the `tasks` list; children
  are stateless views that render what they're given and report user intent.
- **Re-rendering after state changes** — mutating a list item in place doesn't automatically
  refresh the UI, so `StateHasChanged()` tells Blazor to re-render with the updated data.
- **`EventCallback` is async-friendly** — invoking it returns a `Task`, integrating cleanly
  with `@onclick` and automatically triggering a render of the receiving component.
