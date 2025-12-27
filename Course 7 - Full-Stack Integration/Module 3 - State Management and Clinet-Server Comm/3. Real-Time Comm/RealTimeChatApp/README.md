# NovaChat

A real-time chat application built with Blazor and SignalR.

## Project Structure

```
RealTimeChatApp/
├── RealTimeChatApp.Server    # Blazor Server + SignalR Hub
├── RealTimeChatApp.Client    # Blazor WebAssembly (Chat UI)
└── RealTimeChatApp.Shared    # Shared models
```

## Technologies

- .NET 10
- Blazor WebAssembly (Client)
- Blazor Server (Dashboard)
- SignalR (Real-time communication)
- CSS Isolation

## Getting Started

### Prerequisites

- .NET 10 SDK

### Running the Application

1. Start the Server (SignalR Hub + Dashboard):
```bash
cd RealTimeChatApp.Server
dotnet run --urls "http://localhost:5002"
```

2. Start the Client (Chat UI):
```bash
cd RealTimeChatApp.Client
dotnet run --urls "http://localhost:5010"
```

3. Open http://localhost:5010 in your browser

## Features

- Real-time messaging via SignalR WebSocket
- User authentication (name-based)
- Connection state management (connected/reconnecting/disconnected)
- Server dashboard with live statistics
- Responsive UI design

## Project Components

### Server
- **ChatHub** - SignalR hub for message broadcasting
- **ChatStatisticsService** - Tracks connected users and message count
- **Dashboard** - Displays real-time server statistics

### Client
- **Home** - Landing page
- **Login** - User name entry
- **Chat** - Main chat interface
- **ChatService** - SignalR client connection management
- **UserStateService** - User session state

## SignalR Endpoint

```
ws://localhost:5002/chathub
```
