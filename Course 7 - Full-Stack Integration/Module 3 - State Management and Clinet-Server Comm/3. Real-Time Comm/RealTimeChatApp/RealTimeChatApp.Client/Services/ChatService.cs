using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using RealTimeChatApp.Shared.Models;

namespace RealTimeChatApp.Client.Services;

public class ChatService : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly List<ChatMessage> _messages = new();

    public IReadOnlyList<ChatMessage> Messages => _messages;
    public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;
    public HubConnectionState ConnectionState => _hubConnection.State;

    public event Action? OnMessagesChanged;
    public event Action? OnConnectionStateChanged;
    public event Action<string>? OnUserConnected;
    public event Action<string>? OnUserDisconnected;

    public ChatService(IConfiguration configuration)
    {
        var chatHubUrl = configuration["ApiSettings:ChatHubUrl"]
            ?? throw new InvalidOperationException("ChatHubUrl is not configured");

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(chatHubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<ChatMessage>("ReceiveMessage", message =>
        {
            _messages.Add(message);
            OnMessagesChanged?.Invoke();
        });

        _hubConnection.On<string>("UserConnected", connectionId =>
        {
            OnUserConnected?.Invoke(connectionId);
        });

        _hubConnection.On<string>("UserDisconnected", connectionId =>
        {
            OnUserDisconnected?.Invoke(connectionId);
        });

        _hubConnection.Reconnecting += _ =>
        {
            OnConnectionStateChanged?.Invoke();
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += _ =>
        {
            OnConnectionStateChanged?.Invoke();
            return Task.CompletedTask;
        };

        _hubConnection.Closed += _ =>
        {
            OnConnectionStateChanged?.Invoke();
            return Task.CompletedTask;
        };
    }

    public async Task StartAsync()
    {
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
            OnConnectionStateChanged?.Invoke();
        }
    }

    public async Task SendMessageAsync(string user, string message)
    {
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(message))
            return;

        var chatMessage = new ChatMessage
        {
            User = user,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        await _hubConnection.SendAsync("SendMessage", chatMessage);
    }

    public async ValueTask DisposeAsync()
    {
        await _hubConnection.DisposeAsync();
    }
}
