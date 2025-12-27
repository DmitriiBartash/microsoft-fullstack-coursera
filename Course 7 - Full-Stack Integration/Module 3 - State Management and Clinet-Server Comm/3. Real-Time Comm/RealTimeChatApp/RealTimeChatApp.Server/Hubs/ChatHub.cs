using Microsoft.AspNetCore.SignalR;
using RealTimeChatApp.Server.Services;
using RealTimeChatApp.Shared.Models;

namespace RealTimeChatApp.Server.Hubs;

public class ChatHub : Hub
{
    private readonly ChatStatisticsService _statistics;

    public ChatHub(ChatStatisticsService statistics)
    {
        _statistics = statistics;
    }

    /// <summary>
    /// Broadcasts a message to all connected clients.
    /// </summary>
    public async Task SendMessage(ChatMessage message)
    {
        message.Timestamp = DateTime.UtcNow;
        _statistics.MessageSent();
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    public override async Task OnConnectedAsync()
    {
        _statistics.UserConnected();
        await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _statistics.UserDisconnected();
        await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
