namespace RealTimeChatApp.Server.Services;

/// <summary>
/// Tracks chat statistics: connected users and message count.
/// </summary>
public class ChatStatisticsService
{
    private int _connectedUsers;
    private int _totalMessages;
    private readonly object _lock = new();

    public int ConnectedUsers => _connectedUsers;
    public int TotalMessages => _totalMessages;

    public event Action? OnStatisticsChanged;

    public void UserConnected()
    {
        lock (_lock)
        {
            _connectedUsers++;
        }
        NotifyChanged();
    }

    public void UserDisconnected()
    {
        lock (_lock)
        {
            if (_connectedUsers > 0)
                _connectedUsers--;
        }
        NotifyChanged();
    }

    public void MessageSent()
    {
        lock (_lock)
        {
            _totalMessages++;
        }
        NotifyChanged();
    }

    private void NotifyChanged()
    {
        OnStatisticsChanged?.Invoke();
    }
}
