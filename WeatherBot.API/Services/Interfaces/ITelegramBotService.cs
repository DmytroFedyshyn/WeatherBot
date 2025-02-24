using WeatherBot.API.DTOs;

namespace WeatherBot.API.Services.Interfaces
{
    public interface ITelegramBotService
    {
        Task StartAsync();
        Task SendWeatherMessage(long chatId, string city, double temp, string description, CancellationToken cancellationToken = default);
    }
}
