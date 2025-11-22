using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherStateService
    {
        public WeatherData? CurrentWeather { get; private set; }
        public List<User>? Users { get; private set; }

        public event Action? OnChange;

        public void SetWeather(WeatherData data)
        {
            CurrentWeather = data;
            NotifyStateChanged();
        }

        public void SetUsers(List<User> users)
        {
            Users = users;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
