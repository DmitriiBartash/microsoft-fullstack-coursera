namespace RealTimeChatApp.Client.Services;

/// <summary>
/// Manages user session state across pages.
/// </summary>
public class UserStateService
{
    public string? UserName { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrEmpty(UserName);

    public event Action? OnStateChanged;

    public void Login(string userName)
    {
        UserName = userName;
        OnStateChanged?.Invoke();
    }

    public void Logout()
    {
        UserName = null;
        OnStateChanged?.Invoke();
    }
}
