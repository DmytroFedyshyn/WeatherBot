using WeatherBot.API.Models;

namespace WeatherBot.API.Data.Interfaces
{
    public interface IWeatherHistoryRepository
    {
        Task<int> AddWeatherHistoryAsync(WeatherHistory history);
    }
}
