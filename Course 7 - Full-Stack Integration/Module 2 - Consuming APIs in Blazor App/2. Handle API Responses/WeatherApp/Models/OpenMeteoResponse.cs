using System.Text.Json.Serialization;

namespace WeatherApp.Models
{
    public class OpenMeteoResponse
    {
        [JsonPropertyName("current_weather")]
        public CurrentWeather? CurrentWeather { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("windspeed")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("weathercode")]
        public int? WeatherCode { get; set; }
    }
}
