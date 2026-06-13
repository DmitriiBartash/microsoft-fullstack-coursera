# Implementing Real-Time Communication Using SignalR

**Course 7 — Full-Stack Integration** · Module 3 · Lesson 3 · `You Try It!`

> Build a real-time chat app across a **Blazor Server** host, a **Blazor WebAssembly**
> client, and a **shared** class library. A SignalR **hub** broadcasts every message to all
> connected clients, so a note typed in one browser tab appears instantly in the others —
> no refresh, no polling.

---

## 🎯 Objective

By the end of this lab you will be able to implement real-time communication in a full-stack
application using **SignalR**. You will configure SignalR, create a **hub** for communication,
and test live updates in a client-server environment.

---

## 🗂️ What you will build

A solution named **`RealTimeChatApp`** made of three projects plus a SignalR hub:

| Project / file                      | Responsibility                                                        |
| ----------------------------------- | -------------------------------------------------------------------- |
| `Server` (Blazor Server)            | Hosts the SignalR hub, applies CORS, maps `/chathub`                 |
| `Client` (Blazor WebAssembly)       | Chat UI that sends and receives messages live                        |
| `Shared` (class library)            | The `ChatMessage` model shared by both ends                          |
| `Hubs/ChatHub.cs`                   | Broadcasts each `SendMessage` to **all** clients as `ReceiveMessage` |
| `ChatService.cs`                    | Wraps the SignalR `HubConnection` for the UI                         |
| `Pages/Chat.razor`                  | Input box + live message list, bound to `ChatService`               |

**Flow:** `Chat.razor → ChatService.SendMessage → ChatHub.SendMessage → Clients.All "ReceiveMessage" → every Chat.razor`

---

## ✅ Prerequisites

- .NET SDK installed — check with `dotnet --version`
- Visual Studio Code (with the C# / C# Dev Kit extension)
- NuGet packages: `Microsoft.AspNetCore.SignalR` (server) and `Microsoft.AspNetCore.SignalR.Client` (client)

---

## 🛠️ Steps

### Step 1 — Prepare the application

You will create a real-time chat application using Visual Studio Code. This sets up the
server, client, and shared components required for the later steps.

Open the terminal in Visual Studio Code and **create the solution**:

```bash
mkdir RealTimeChatApp
cd RealTimeChatApp
dotnet new sln
```

**Create the Server project** (Blazor Server) and add it to the solution:

```bash
dotnet new blazor -n Server
dotnet sln add Server/Server.csproj
```

**Create the Client project** (Blazor WebAssembly) and add it to the solution:

```bash
dotnet new blazorwasm -n Client
dotnet sln add Client/Client.csproj
```

**Create the Shared project** (class library for shared models) and add it:

```bash
dotnet new classlib -n Shared
dotnet sln add Shared/Shared.csproj
```

**Reference the Shared project** from both the server and the client:

```bash
dotnet add Server/Server.csproj reference Shared/Shared.csproj
dotnet add Client/Client.csproj reference Shared/Shared.csproj
```

**Install SignalR** — the hub package on the server, the client package on the client:

```bash
dotnet add Server/Server.csproj package Microsoft.AspNetCore.SignalR
dotnet add Client/Client.csproj package Microsoft.AspNetCore.SignalR.Client
```

**Verify the setup** by building the whole solution:

```bash
dotnet build
```

### Step 2 — Implement a shared model

Create a shared model for consistent data handling between the client and server.

- In the `Shared` project, create a folder named `Models`.
- Add a new file named `ChatMessage.cs` inside `Models`.
- Define the `ChatMessage` class with properties for **User**, **Message**, and **Timestamp**.

```csharp
namespace Shared.Models;

public class ChatMessage
{
    public string User { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

Because both the server and the client reference `Shared`, they serialize and deserialize the
*same* type — no shape drift between the two ends.

### Step 3 — Create a SignalR hub

Create a SignalR hub to manage real-time communication between the server and connected clients.

- In the `Server` project, create a folder named `Hubs`.
- Add a new file named `ChatHub.cs` inside `Hubs`.
- Define a `ChatHub` class that broadcasts messages to **all** connected clients.

```csharp
using Microsoft.AspNetCore.SignalR;
using Shared.Models;

namespace Server.Hubs;

public class ChatHub : Hub
{
    // Broadcasts an incoming message to every connected client.
    public async Task SendMessage(ChatMessage message)
    {
        message.Timestamp = DateTime.UtcNow;
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
```

`SendMessage` is the **client → server** call; `"ReceiveMessage"` is the **server → all clients**
broadcast the UI listens for.

### Step 4 — Configure the server

Set up the SignalR hub in the server so the client and server can communicate.

- Open `Program.cs` in the `Server` project.
- Register SignalR, add a **CORS** policy that allows the client origin (with credentials),
  and map the hub to `/chathub`.

```csharp
using Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register the SignalR services.
builder.Services.AddSignalR();

// Allow the Blazor WebAssembly client to call the hub (credentials required for SignalR).
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7002", "http://localhost:5002")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseCors("CorsPolicy");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Expose the hub endpoint the client connects to.
app.MapHub<ChatHub>("/chathub");

app.Run();
```

> **Note:** match the origins above to the URLs your client actually runs on
> (see them printed by `dotnet run`, or in `Client/Properties/launchSettings.json`).

### Step 5 — Build the client interface

Create a simple chat interface in the client project to send and receive messages, and register
the `ChatService` in the DI container.

First, create a new file `ChatService.cs` in the **root** of the `Client` project. It opens the
`HubConnection`, listens for `"ReceiveMessage"`, and exposes `SendMessageAsync` to the UI:

```csharp
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Models;

namespace Client;

public class ChatService : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly List<ChatMessage> _messages = new();

    public IReadOnlyList<ChatMessage> Messages => _messages;
    public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

    public event Action? OnMessagesChanged;

    public ChatService()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7001/chathub")
            .WithAutomaticReconnect()
            .Build();

        // Server → client broadcast handler.
        _hubConnection.On<ChatMessage>("ReceiveMessage", message =>
        {
            _messages.Add(message);
            OnMessagesChanged?.Invoke();
        });
    }

    public async Task StartAsync()
    {
        if (_hubConnection.State == HubConnectionState.Disconnected)
            await _hubConnection.StartAsync();
    }

    public async Task SendMessageAsync(string user, string message)
    {
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(message))
            return;

        var chatMessage = new ChatMessage { User = user, Message = message };
        await _hubConnection.SendAsync("SendMessage", chatMessage);
    }

    public async ValueTask DisposeAsync() => await _hubConnection.DisposeAsync();
}
```

> Point `WithUrl(...)` at the **server's** HTTPS URL + `/chathub` — the host you configured in Step 4,
> not the client's own address.

**Register the `ChatService` in the DI container.** Open `Program.cs` in the `Client` project and
add it as a singleton:

```csharp
builder.Services.AddSingleton<ChatService>();
```

**Create the chat interface.** In the `Client` project, go to the `Pages` folder and create a new
Razor component named `Chat.razor`. Use `@inject` to pull in the `ChatService`, accept user input,
and render messages dynamically:

```razor
@page "/chat"
@using Shared.Models
@inject ChatService ChatService
@implements IDisposable

<h3>General Chat</h3>

<div class="messages">
    @if (!ChatService.Messages.Any())
    {
        <p><em>No messages yet. Be the first to say hi!</em></p>
    }
    else
    {
        @foreach (var msg in ChatService.Messages)
        {
            <div class="message">
                <strong>@msg.User</strong>
                <span class="time">@msg.Timestamp.ToLocalTime().ToString("HH:mm")</span>
                <div>@msg.Message</div>
            </div>
        }
    }
</div>

<div class="composer">
    <input @bind="_user" @bind:event="oninput" placeholder="Your name" />
    <input @bind="_message" @bind:event="oninput"
           @onkeypress="HandleKeyPress" placeholder="Type a message..." />
    <button @onclick="Send" disabled="@(!ChatService.IsConnected)">Send</button>
</div>

@code {
    private string _user = string.Empty;
    private string _message = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        ChatService.OnMessagesChanged += OnMessagesChanged;
        await ChatService.StartAsync();
    }

    private async Task Send()
    {
        await ChatService.SendMessageAsync(_user, _message);
        _message = string.Empty;
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            await Send();
    }

    private void OnMessagesChanged() => InvokeAsync(StateHasChanged);

    public void Dispose() => ChatService.OnMessagesChanged -= OnMessagesChanged;
}
```

The `@inject` directive binds the interface to the `ChatService`; the `OnMessagesChanged` event
triggers `StateHasChanged()` so each new broadcast re-renders the list automatically.

### Step 6 — Test real-time communication

Run and test the application to ensure messages are synchronized across all clients.

**Start the server** project:

```bash
dotnet run --project Server/Server.csproj
```

**Start the client** project (in a second terminal):

```bash
dotnet run --project Client/Client.csproj
```

- Open the client application in **two** browser tabs and navigate to `/chat`.
- Send a message from one tab and verify it appears in the other tab **in real time**.

---

## ▶️ Expected result

Both tabs are connected to the hub. A message sent from one tab is broadcast by `ChatHub` and
**appears instantly in every open tab** — sender name, text, and timestamp — with no page refresh.

---

## ☑️ Definition of done

- [ ] `RealTimeChatApp` solution holds `Server`, `Client`, and `Shared`, all building with `dotnet build`
- [ ] Both projects reference `Shared`; SignalR server + client packages are installed
- [ ] `Shared/Models/ChatMessage.cs` defines `User`, `Message`, and `Timestamp`
- [ ] `Server/Hubs/ChatHub.cs` broadcasts `SendMessage` to all clients as `ReceiveMessage`
- [ ] Server `Program.cs` registers SignalR + CORS and maps `MapHub<ChatHub>("/chathub")`
- [ ] `ChatService` connects to the hub; `Chat.razor` injects it and is registered as a singleton
- [ ] A message sent in one browser tab appears live in another

---

## 🔑 Key concepts

- **A hub is the contract** — clients call server methods (`SendMessage`) and the server pushes to
  clients by name (`Clients.All.SendAsync("ReceiveMessage", …)`); both ends just agree on the strings.
- **Shared model = one source of truth** — putting `ChatMessage` in the `Shared` library means server
  and client serialize the identical type, eliminating drift between the two ends.
- **Persistent connection, not polling** — `HubConnection` holds an open channel (WebSockets when
  available), so the server can *push* updates the instant they happen instead of the client asking.
- **CORS + credentials are mandatory for cross-origin SignalR** — the WebAssembly client lives on a
  different origin than the hub, so the policy must list that origin and `AllowCredentials()`.
- **Resilience for free** — `WithAutomaticReconnect()` re-establishes the connection after a drop,
  which is why the UI gates sending on `IsConnected`.
