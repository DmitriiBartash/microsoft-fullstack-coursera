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
