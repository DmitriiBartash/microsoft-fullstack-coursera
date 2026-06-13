# Implementing Hybrid Rendering

**Course 4 — Blazor for Front-End Development** · Module 4 · Lesson 2 · `You Try It!`

> Build a Blazor app that mixes **server-side** and **client-side** render modes in one
> page, then measure and tune it. You'll combine **SSR**, **Interactive Server**, and
> **Interactive Auto (WebAssembly)** components, add `ILogger` timing around the
> component lifecycle, and apply **lazy loading** so heavy UI is rendered only on demand.

---

## 🎯 Objective

By the end of this lab you will be able to **implement hybrid rendering** in Blazor by
composing components with different `@rendermode` values, **analyze performance** with
`Stopwatch` and `ILogger` instrumentation, and **apply optimization strategies** — chiefly
lazy loading — to improve responsiveness.

---

## 🗂️ What you will build

A Blazor app named **`BlazorPerformanceApp`** whose home page hosts several components, each
running under a different render mode so you can compare their behaviour side by side.

| Component             | Render mode             | Responsibility                                        |
| --------------------- | ----------------------- | ----------------------------------------------------- |
| `SSRPane.razor`       | Static SSR              | Renders mock data once on the server (no interactivity)|
| `ServerClock.razor`   | `InteractiveServer`     | Live server-driven clock via a `System.Timers.Timer`  |
| `SlowList.razor`      | `InteractiveAuto`       | Heavy list (WASM) used to expose render cost          |
| `HybridComponent.razor`| `InteractiveServer`    | Combines all panes + logs lifecycle timings           |
| `Home.razor`          | `InteractiveServer`     | Landing page that **lazy-loads** the hybrid component  |

**Flow:** `Home → click "Load Hybrid Inline" → HybridComponent renders → OnInitialized → OnInitializedAsync → OnAfterRender (timings logged)`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version` (use a current LTS such as .NET 8/9, since `@rendermode` and `InteractiveAuto` require the Blazor Web App model)
- Visual Studio Code with the **C# Dev Kit** extension
- A browser to open the running app

---

## 🛠️ Steps

### Step 1 — Prepare the application

Create the project, open it, and confirm the default app runs. This app becomes the
foundation for the render-mode and performance work that follows.

```bash
dotnet new blazorwasm -o BlazorPerformanceApp
cd BlazorPerformanceApp
code .
dotnet run
```

- Copy the URL shown in the terminal (e.g. `http://localhost:5000`) and open it in your browser.
- Verify the default Blazor application loads successfully.
- In the `Pages` folder, review the default Razor components and locate **`Home.razor`** — this is the file you will modify in the next steps.

> *Note:* the `@rendermode` directive, `AddInteractiveServerComponents`, and `InteractiveAuto`
> used below are features of the **Blazor Web App** project model (.NET 8+). If your template
> only produces a pure WebAssembly app, create the project with the interactive Blazor Web App
> template instead so server and auto render modes are available:
>
> ```bash
> dotnet new blazor -o BlazorPerformanceApp --interactivity Auto
> ```

### Step 2 — Implement hybrid rendering

Add hybrid rendering by combining server-side and client-side Blazor features inside a new
`HybridComponent`, using mock data to simulate the scenario.

- In the `Pages` folder, create a new Razor component named **`HybridComponent.razor`**.
- Add `HybridComponent` to `Home.razor` so you can confirm it renders.
- Register the interactive server services in **`Program.cs`**. After `builder` is declared and
  **before** `await builder.Build().RunAsync();`, add:

```csharp
builder.Services.AddInteractiveServerComponents();
```

The supporting panes used by the hybrid component are defined here. Create each file in the
`Components` (or `Pages`) folder.

**`SSRPane.razor`** — static, server-rendered mock data:

```razor
@code {
    private readonly DateTime _renderedAt = DateTime.UtcNow;
    private readonly string[] _mock = new[] { "Alpha", "Bravo", "Charlie", "Delta" };
}

<div class="element">
    <p>Rendered at (UTC): @_renderedAt:HH:mm:ss</p>
    <ul>
        @foreach (var s in _mock)
        {
            <li>@s</li>
        }
    </ul>
</div>
```

**`ServerClock.razor`** — interactive server clock that ticks once a second and disposes its timer:

```razor
@using System.Timers
@implements IDisposable

<div class="element">
    <p>Server time</p>
    <h3>@_now.ToLongTimeString()</h3>
</div>

@code {
    private DateTime _now = DateTime.Now;
    private Timer? _timer;

    protected override void OnInitialized()
    {
        _timer = new Timer(1000);
        _timer.Elapsed += (_, __) =>
        {
            _now = DateTime.Now;
            InvokeAsync(StateHasChanged);
        };
        _timer.Start();
    }

    public void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }
}
```

**`SlowList.razor`** — a deliberately heavy list (runs as WASM via `InteractiveAuto`) to make render cost visible:

```razor
@code {
    [Parameter] public int ItemsCount { get; set; } = 50;
    private List<string> _items = new();

    protected override void OnInitialized()
    {
        for (int i = 0; i < ItemsCount; i++)
        {
            _items.Add($"Item {i:D3}");
        }
    }

    private void Shuffle()
    {
        var rnd = new Random();
        for (int i = _items.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            (_items[i], _items[j]) = (_items[j], _items[i]);
        }
    }
}

<div class="element">
    <button class="btn" @onclick="Shuffle">Shuffle</button>
    <ul>
        @foreach (var it in _items)
        {
            <li>@it</li>
        }
    </ul>
</div>
```

### Step 3 — Monitor and analyze performance

Use .NET's built-in diagnostics to measure how the component behaves. Inject `ILogger` into
`HybridComponent`, start a `Stopwatch` in `OnInitialized`, and log key lifecycle events so you
can read the elapsed milliseconds in the console.

**`HybridComponent.razor`** — composes the panes under three different render modes and logs timings:

```razor
@using System.Diagnostics
@page "/hybrid"
@rendermode InteractiveServer
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@using BlazorPerf.Components
@using BlazorPerf.Client.Components
@inject ILogger<HybridComponent> Logger

<PageTitle>Hybrid Component</PageTitle>

<section class="hybrid">
  <header class="hybrid__head">
    <h1>Hybrid Component</h1>
  </header>
  <div class="cards">
    <div class="card">
      <h3>SSR-only</h3>
      <SSRPane />
    </div>
    <div class="card">
      <h3>Interactive Server</h3>
      <ServerClock @rendermode="InteractiveServer" />
    </div>
    <div class="card">
      <h3>Interactive Auto (WASM)</h3>
      <SlowList @rendermode="InteractiveAuto" ItemsCount="300" />
    </div>
  </div>
</section>

@code {
    private readonly Stopwatch _sw = new();

    protected override void OnInitialized()
    {
        _sw.Start();
        Logger.LogInformation("HybridComponent OnInitialized at {Utc}", DateTime.UtcNow);
    }

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("HybridComponent OnInitializedAsync start at {Utc}", DateTime.UtcNow);
        await Task.Yield();
        Logger.LogInformation("HybridComponent OnInitializedAsync end at {Utc} (elapsed {Ms} ms)", DateTime.UtcNow, _sw.ElapsedMilliseconds);
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            Logger.LogInformation("HybridComponent first render complete at {Utc} (elapsed {Ms} ms)", DateTime.UtcNow, _sw.ElapsedMilliseconds);
        return Task.CompletedTask;
    }
}
```

### Step 4 — Apply optimization techniques

Optimize `HybridComponent` for performance by adding **lazy loading**: instead of rendering the
component on page load, render it only when the user clicks a button.

- Update `Home.razor` to replace the direct inclusion of `HybridComponent` with a lazy-loading
  approach guarded by a flag.

**`Home.razor`** — lazy-loads the hybrid component behind a button:

```razor
@page "/"
@rendermode InteractiveServer
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@using BlazorPerf.Components
@using BlazorPerf.Client.Components
@inject NavigationManager Nav

<PageTitle>Home</PageTitle>

<section class="hero">
  <div class="hero__content">
    <h1 class="title">Blazor Hybrid Rendering Lab</h1>
    <p class="subtitle">SSR • Interactive Server • Interactive WebAssembly</p>
    <button class="btn" @onclick="LoadHybrid">Load Hybrid Inline</button>
    <a class="btn btn--ghost" href="/hybrid" style="margin-left:8px;">Open Full Page</a>
  </div>
</section>

<section class="preview">
  <h2 class="preview__title">Live Preview</h2>
  <div class="cards">
    <div class="card">
      <h3>SSR-only</h3>
      <SSRPane />
    </div>
    <div class="card">
      <h3>Interactive Server</h3>
      <ServerClock @rendermode="InteractiveServer" />
    </div>
  </div>
</section>

@if (showHybrid)
{
  <section class="lazy" id="hybrid-host">
    <h2 class="preview__title">Hybrid (lazy)</h2>
    <article class="lazy__host">
      <HybridComponent />
    </article>
  </section>
}

@code {
    private bool showHybrid;

    private async Task LoadHybrid()
    {
        if (!showHybrid) { showHybrid = true; await Task.Yield(); }
        Nav.NavigateTo("#hybrid-host");
    }
}
```

- Rebuild and run the application, then click the **"Load Hybrid Inline"** button to verify the
  lazy-loading functionality works.

```bash
dotnet run
```

### Step 5 — Validate and reassess

Re-evaluate the application after the optimization to confirm performance has improved.

- **Rerun performance measurements:** observe the app's behaviour before and after clicking the
  **"Load Hybrid Inline"** button, and compare the logged `OnInitialized` / `OnInitializedAsync` /
  first-render timings.
- **Document observations:** note improvements in responsiveness, and explain how lazy loading
  improves performance by **deferring resource usage until it is actually required**.

---

## ▶️ Expected result

The home page loads quickly with only the lightweight SSR and Interactive Server panes visible.
Clicking **"Load Hybrid Inline"** renders `HybridComponent` on demand and scrolls to it, and the
console shows ordered log lines such as `HybridComponent OnInitialized…`, `OnInitializedAsync
start/end (elapsed N ms)`, and `first render complete (elapsed N ms)` — evidence that the heavy
component's cost was deferred until the click.

---

## ☑️ Definition of done

- [ ] `BlazorPerformanceApp` project created and runs in the browser
- [ ] `Program.cs` calls `builder.Services.AddInteractiveServerComponents();`
- [ ] `HybridComponent.razor` composes `SSRPane`, `ServerClock` (`InteractiveServer`) and `SlowList` (`InteractiveAuto`, `ItemsCount="300"`)
- [ ] `ILogger` + `Stopwatch` log `OnInitialized`, `OnInitializedAsync`, and first-render timings
- [ ] `Home.razor` renders `HybridComponent` only after the **"Load Hybrid Inline"** button is clicked
- [ ] `dotnet run` shows the page loading fast and the component appearing on demand with timing logs

---

## 🔑 Key concepts

- **Hybrid rendering = per-component render modes** — one page can mix static SSR, `InteractiveServer`, and `InteractiveAuto` so each component pays only for the interactivity it needs.
- **Measure before you tune** — `Stopwatch` + `ILogger` around `OnInitialized` / `OnInitializedAsync` / `OnAfterRenderAsync` turn "feels slow" into concrete elapsed-millisecond numbers.
- **Lazy loading defers cost** — gating the heavy `HybridComponent` behind a flag/button keeps initial load light and only spends render time when the user asks for it.
- **Clean up interactive resources** — server-driven components like `ServerClock` must `Dispose` their `Timer` (via `@implements IDisposable`) to avoid leaks and runaway callbacks.
- **Render mode drives where code runs** — `InteractiveServer` executes over a SignalR circuit, while `InteractiveAuto` can run the same component in WebAssembly; the choice changes latency, payload, and CPU location.
