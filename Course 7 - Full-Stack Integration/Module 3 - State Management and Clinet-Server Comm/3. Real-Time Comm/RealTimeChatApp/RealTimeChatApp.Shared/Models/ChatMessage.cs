namespace RealTimeChatApp.Shared.Models;

public class ChatMessage
{
    public string User { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
