using WeatherBot.API.DTOs;

namespace WeatherBot.API.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherResponse?> GetWeatherAsync(string city);
    }
}
