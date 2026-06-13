# Setting Up and Debugging a Blazor Project

**Course 4 — Blazor for Front-End Development** · Module 1 · `You Try It!`

> Scaffold a **Blazor WebAssembly** app in Visual Studio Code, then debug it three
> different ways: **`Console.WriteLine` print statements**, the VS Code **debugger**
> (breakpoints + the Watch window), and **Hot Reload** for instant UI updates — all on
> the stock `Counter.razor` page.

---

## 🎯 Objective

By the end of this activity you will be able to **run and debug Blazor applications using
Visual Studio Code**. You will set up a project, use debugging tools like **breakpoints**,
the **Watch window**, and **Hot Reload**, and identify issues in your code efficiently.

---

## 🗂️ What you'll build

A Blazor WebAssembly project named **`MyBlazorApp`** that you debug with three techniques:

| Technique                | Tooling                                   | What it shows you                                   |
| ------------------------ | ----------------------------------------- | --------------------------------------------------- |
| Print-statement debugging| `Console.WriteLine` + VS Code terminal    | The value of `currentCount` on each loop iteration  |
| Interactive debugging    | C# Dev Kit breakpoints + Watch window     | Step through `IncrementCount`, inspect live state   |
| Hot Reload               | `dotnet watch run`                        | UI text changes reflected without a restart         |

**Flow:** `dotnet new blazorwasm  →  code .  →  set breakpoints / WriteLine  →  dotnet watch run  →  observe`

---

## ✅ Prerequisites

- **.NET SDK** — minimum version **6.0** (check with `dotnet --version`)
- **Visual Studio Code**
- The **C# Dev Kit** extension — required to debug Blazor applications

> The C# Dev Kit ships the debugger and launch-configuration support that the built-in
> C# experience alone does not provide.

---

## 🛠️ Steps

### Step 1 — Prepare the application

Install the C# Dev Kit extension first, then scaffold the project.

1. Open Visual Studio Code.
2. Open the **Extensions** view (the Extensions icon in the Activity Bar).
3. Search for **C# Dev Kit** and click **Install**.
4. **Restart** Visual Studio Code so the extension activates correctly.

Create a project directory, scaffold a Blazor WebAssembly app, and open it:

```bash
mkdir MyBlazorApp
cd MyBlazorApp
dotnet new blazorwasm -o MyBlazorApp
code .
```

Add a launch configuration so VS Code can start and attach the debugger. Create
**`.vscode/launch.json`**:

```jsonc
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Launch and Debug Blazor WebAssembly",
      "type": "blazorwasm",
      "request": "launch",
      "cwd": "${workspaceFolder}"
    }
  ]
}
```

- Press **F5** to build, launch the app in the browser, and attach the debugger.

### Step 2 — Debug with print statements

> **Scenario:** a `for` loop in your Blazor application isn't iterating as expected. Use
> print statements to debug the issue.

1. Open **`Counter.razor`** — find it in the **`Pages`** folder of your application.
2. Replace the body of the **`IncrementCount`** method with a `for` loop that increments
   the `currentCount` variable by one, **5 times**.
3. Add **`Console.WriteLine`** statements that print the variables to the console.
4. When the Blazor app launches, select the **Counter** page from the user interface.
5. Select the **Click me** button.
6. Inspect the variables from the `Console.WriteLine` statement in the **VS Code terminal**.

The relevant section of `@code` looks like this:

```csharp
private int currentCount = 0;

private void IncrementCount()
{
    for (int i = 0; i < 5; i++)
    {
        currentCount++;
        Console.WriteLine($"Loop iteration {i + 1}, currentCount = {currentCount}");
    }
}
```

> **Tip:** to step through the same loop interactively, set a breakpoint on the
> `currentCount++;` line and add `currentCount` and `i` to the **Watch** window — you'll
> see both values update on every pass.

### Step 3 — Use Hot Reload for instant updates

> **Scenario:** you want to modify the UI text and see changes reflected immediately
> without restarting the application.

Start the application with Hot Reload enabled:

```bash
dotnet watch run
```

1. Modify the text displayed in **`Counter.razor`** — for example, change the button text
   or the title.
2. **Save** the changes and observe the updated UI in the browser **instantly**, with no
   restart.

Here is the complete `Counter.razor` after the edits in Steps 2–3:

```razor
@page "/counter"

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<p role="status">Current count: @currentCount</p>

<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        for (int i = 0; i < 5; i++)
        {
            currentCount++;
            Console.WriteLine($"Loop iteration {i + 1}, currentCount = {currentCount}");
        }
    }
}
```

---

## ▶️ Expected result

Clicking **Click me** increases **Current count** by **5** each time (the loop runs five
iterations). The VS Code terminal prints one line per iteration, e.g.:

```text
Loop iteration 1, currentCount = 1
Loop iteration 2, currentCount = 2
Loop iteration 3, currentCount = 3
Loop iteration 4, currentCount = 4
Loop iteration 5, currentCount = 5
```

Editing text in `Counter.razor` while `dotnet watch run` is active updates the browser
immediately — confirming Hot Reload is working.

---

## ☑️ Definition of done

- [ ] C# Dev Kit installed and VS Code restarted
- [ ] `MyBlazorApp` Blazor WebAssembly project created with `dotnet new blazorwasm` and opened in VS Code
- [ ] `.vscode/launch.json` lets you launch and debug the app with **F5**
- [ ] `IncrementCount` uses a 5-iteration `for` loop with `Console.WriteLine` output
- [ ] Iteration values appear in the VS Code terminal when **Click me** is pressed
- [ ] `dotnet watch run` reflects a `Counter.razor` text edit in the browser without a restart

---

## 🔑 Key concepts

- **Blazor WebAssembly scaffolding** — `dotnet new blazorwasm` produces a runnable
  single-page app; `code .` opens it where the C# Dev Kit can debug it.
- **Three complementary debugging tools** — print statements for a quick trace,
  breakpoints + the Watch window for live inspection, and Hot Reload for fast UI iteration.
- **Print-statement tracing** — `Console.WriteLine` inside the loop surfaces `currentCount`
  on every pass, making an unexpected iteration count obvious in the terminal.
- **Hot Reload** — `dotnet watch run` recompiles and applies edits on save, so UI tweaks
  show up instantly without losing the running session.
- **C# Dev Kit is the enabler** — Blazor debugging (breakpoints, Watch, launch configs)
  depends on the extension being installed and active.
