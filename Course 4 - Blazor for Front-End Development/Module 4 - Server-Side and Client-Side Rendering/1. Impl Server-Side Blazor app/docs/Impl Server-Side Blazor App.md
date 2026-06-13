# Impl Server-Side Blazor App

**Course 4 — Blazor for Front-End Development** · Module 4 · Lesson 1 · `You Try It!`

> Build a **server-side Blazor** application that adds two interactive features on top of
> the default template: a **real-time chat** powered by **SignalR**, and a page that
> demonstrates **server-side state management** through an injected singleton. You'll
> wire up a hub, two `InteractiveServer` pages, and extend the navigation menu so both
> features are reachable.

---

## 🎯 Objective

By the end of this lab you will create a server-side Blazor application using Visual Studio
Code. The activity demonstrates **real-time communication with SignalR** and **state
management** in a Blazor Server app, and you will modify the default navigation to include
links to your new pages.

---

## 🗂️ What you will build

A Blazor Server project named **`BlazorServerApp`** with these new pieces layered onto the
template:

| File / Component                     | Responsibility                                                        |
| ------------------------------------ | --------------------------------------------------------------------- |
| `Hubs/NotificationHub.cs`            | SignalR hub that broadcasts a user's message to all connected clients |
| `Services/StateContainer.cs`         | Singleton holding a counter + `OnChange` event for shared state       |
| `Components/Pages/SignalRChat.razor` | Interactive chat UI that sends/receives messages live via the hub     |
| `Components/Pages/StateManagement.razor` | Page that reads and increments the shared counter                 |
| `Components/Layout/NavMenu.razor`    | Navigation extended with links to the two new pages                   |
| `Program.cs`                         | Registers SignalR, the `StateContainer`, and maps the hub endpoint    |

**Flow:** `Client (SignalRChat) → hub.SendAsync("SendMessage") → NotificationHub → Clients.All.SendAsync("ReceiveMessage") → every connected client`

---

## ✅ Prerequisites

- .NET SDK installed (Blazor template targets **.NET 8**) — check with `dotnet --version`
- Visual Studio Code with the integrated terminal
- NuGet package: `Microsoft.AspNetCore.SignalR.Client` (version `8.*`)

---

## 🛠️ Steps

### Step 1 — Prepare the application

Set up a server-side Blazor application using the updated `blazor` template.

```bash
# Create the app from the Blazor template
dotnet new blazor -o BlazorServerApp

# Move into the project
cd BlazorServerApp

# Open the folder in VS Code
code .

# Restore dependencies
dotnet restore

# Add the SignalR client package
dotnet add package Microsoft.AspNetCore.SignalR.Client --version 8.*

# Run the application
dotnet run
```

Open your browser at the location indicated in the terminal (e.g. `https://localhost:5001`)
to confirm the app runs correctly.

### Step 2 — Configure real-time features with SignalR

Implement a real-time chat feature using SignalR.

**Server-side setup** — in `Program.cs`, register the SignalR services and map the hub
endpoint. Also register the `StateContainer` here (used in Step 3) so it is available for
the whole app:

```csharp
using BlazorServerApp.Components;
using BlazorServerApp.Hubs;
using BlazorServerApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Razor components with interactive server render mode
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Real-time messaging
builder.Services.AddSignalR();

// Shared server-side state (singleton -> one instance for the whole server)
builder.Services.AddSingleton<StateContainer>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// SignalR hub endpoint
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
```

Create a folder named **`Hubs`** in the root directory. Inside it, create
**`NotificationHub.cs`**. The hub defines a `SendMessage` method that sanitizes the input
and broadcasts it (with a UTC timestamp) to every connected client via the
`ReceiveMessage` event:

```csharp
using Microsoft.AspNetCore.SignalR;

namespace BlazorServerApp.Hubs;

public class NotificationHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        var safeUser = string.IsNullOrWhiteSpace(user) ? "Anonymous" : user.Trim();
        var safeMsg  = message?.Trim() ?? string.Empty;

        await Clients.All.SendAsync(
            "ReceiveMessage",
            safeUser,
            safeMsg,
            DateTimeOffset.UtcNow
        );
    }
}
```

**Client-side integration** — navigate to the `Components/Pages` folder and create a new
Razor component named **`SignalRChat.razor`**. It builds a `HubConnection`, listens for
`ReceiveMessage`, and lets users send messages dynamically without a page refresh:

```razor
@page "/signalr-chat"
@rendermode InteractiveServer
@using Microsoft.AspNetCore.SignalR.Client
@inject NavigationManager Nav

<h3>SignalR Chat</h3>

<div class="mb-3">
    <label class="form-label">Display name</label>
    <input @bind="userName" @bind:event="oninput" class="form-control" />
</div>
<div class="mb-3">
    <label class="form-label">Message</label>
    <input @bind="message" @bind:event="oninput" class="form-control" @onkeydown="HandleEnter" />
    <button class="btn btn-primary mt-2" @onclick="SendAsync">Send</button>
</div>

<p class="text-muted">Hub state: @hubState</p>

<ul class="list-group">
    @foreach (var m in messages)
    {
        <li class="list-group-item">
            <b>@m.User</b>: @m.Text
            <div class="text-muted" style="font-size:.8rem">@m.When.LocalDateTime</div>
        </li>
    }
</ul>

@code {
    private HubConnection? hub;
    private string? userName;
    private string? message;
    private string hubState = "Not connected";
    private readonly List<ChatMessage> messages = new();

    protected override async Task OnInitializedAsync()
    {
        hub = new HubConnectionBuilder()
            .WithUrl(Nav.ToAbsoluteUri("/notificationHub"))
            .WithAutomaticReconnect()
            .Build();

        hub.Reconnecting += error => { hubState = "Reconnecting..."; StateHasChanged(); return Task.CompletedTask; };
        hub.Reconnected  += id    => { hubState = "Connected";     StateHasChanged(); return Task.CompletedTask; };
        hub.Closed       += error => { hubState = "Closed";         StateHasChanged(); return Task.CompletedTask; };

        hub.On<string, string, DateTimeOffset>("ReceiveMessage", (user, text, when) =>
        {
            messages.Add(new ChatMessage(user, text, when));
            InvokeAsync(StateHasChanged);
        });

        await hub.StartAsync();
        hubState = "Connected";
    }

    private async Task SendAsync()
    {
        if (hub is null) return;
        var u = string.IsNullOrWhiteSpace(userName) ? "Anonymous" : userName!.Trim();
        var t = message?.Trim();
        if (string.IsNullOrWhiteSpace(t)) return;
        await hub.SendAsync("SendMessage", u, t);
        message = string.Empty;
    }

    private async Task HandleEnter(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SendAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (hub is not null)
        {
            await hub.DisposeAsync();
        }
    }

    private record ChatMessage(string User, string Text, DateTimeOffset When);
}
```

### Step 3 — Demonstrate state management

Add a page to demonstrate server-side state management backed by a shared singleton.

First, create a folder named **`Services`** in the root directory and add
**`StateContainer.cs`**. It holds a counter and raises `OnChange` whenever the value
changes, so any subscribed component re-renders:

```csharp
namespace BlazorServerApp.Services;

public class StateContainer
{
    public int Counter { get; private set; }

    public event Action? OnChange;

    public void Increment()
    {
        Counter++;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
```

> The `StateContainer` was registered as a singleton in `Program.cs` in Step 2
> (`builder.Services.AddSingleton<StateContainer>();`).

Now create a new Razor component named **`StateManagement.razor`** in the
`Components/Pages` folder. It injects the container, subscribes to `OnChange`, and
increments the counter on a button click:

```razor
@page "/state-management"
@rendermode InteractiveServer
@inject BlazorServerApp.Services.StateContainer State
@implements IDisposable

<h3>Server-Side State Management</h3>

<p class="lead">
    Counter value: <b>@State.Counter</b>
</p>

<button class="btn btn-success" @onclick="Increment">+1</button>

@code {
    protected override void OnInitialized()
    {
        State.OnChange += StateHasChanged;
    }

    private void Increment() => State.Increment();

    public void Dispose()
    {
        State.OnChange -= StateHasChanged;
    }
}
```

### Step 4 — Update navigation

Modify the navigation menu to include links to the new pages. Open
`Components/Layout/NavMenu.razor` and add two `NavLink` entries that point at the routes
defined on the new components (`/signalr-chat` and `/state-management`):

```razor
<div class="nav-item px-3">
    <NavLink class="nav-link" href="signalr-chat">
        <span class="bi bi-chat-dots-fill-nav-menu" aria-hidden="true"></span> SignalR Chat
    </NavLink>
</div>
<div class="nav-item px-3">
    <NavLink class="nav-link" href="state-management">
        <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> State Management
    </NavLink>
</div>
```

### Step 5 — Test the application

```bash
dotnet run
```

Then navigate to each route and confirm the behavior:

| Route               | What to test                                                              |
| ------------------- | ------------------------------------------------------------------------- |
| `/signalr-chat`     | Real-time chat — open in two browser tabs and watch messages appear live  |
| `/state-management` | State management — click **+1** and observe the counter increment         |

---

## ▶️ Expected result

The app runs and serves both new pages from the navigation menu. On **`/signalr-chat`**,
messages sent from one client appear instantly in every connected client (the hub state
reads `Connected`). On **`/state-management`**, clicking **+1** increases the shared
counter, demonstrating server-side state held by the singleton `StateContainer`.

---

## ☑️ Definition of done

- [ ] `BlazorServerApp` created from the `blazor` template and `Microsoft.AspNetCore.SignalR.Client` (`8.*`) added
- [ ] `Program.cs` calls `AddSignalR()`, registers `StateContainer`, and maps `NotificationHub` at `/notificationHub`
- [ ] `Hubs/NotificationHub.cs` broadcasts `ReceiveMessage` to `Clients.All`
- [ ] `Services/StateContainer.cs` exposes `Counter`, `Increment()`, and the `OnChange` event
- [ ] `SignalRChat.razor` connects to the hub and sends/receives messages without a refresh
- [ ] `StateManagement.razor` increments and displays the shared counter
- [ ] `NavMenu.razor` links to `/signalr-chat` and `/state-management`
- [ ] `dotnet run` serves both pages and they behave as described

---

## 🔑 Key concepts

- **SignalR hubs broadcast in real time** — the hub's `Clients.All.SendAsync("ReceiveMessage", …)` pushes a message to every connected client, so chat updates appear without polling or page refreshes.
- **InteractiveServer render mode** — `@rendermode InteractiveServer` keeps UI events and rendering on the server over a persistent connection, which is what makes the live chat and button-driven counter respond instantly.
- **State via an injected singleton** — registering `StateContainer` with `AddSingleton` gives one shared instance; raising `OnChange` and calling `StateHasChanged` is the pattern for propagating state changes to subscribed components.
- **Always clean up connections and subscriptions** — `SignalRChat` disposes its `HubConnection` and `StateManagement` unsubscribes from `OnChange` in `Dispose`, preventing leaks and stale handlers.
- **Routes drive navigation** — each component declares its own `@page` route, and `NavMenu` `NavLink`s simply reference those routes (`/signalr-chat`, `/state-management`).
