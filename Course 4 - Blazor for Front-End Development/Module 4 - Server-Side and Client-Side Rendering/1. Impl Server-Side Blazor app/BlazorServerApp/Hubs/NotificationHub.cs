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
