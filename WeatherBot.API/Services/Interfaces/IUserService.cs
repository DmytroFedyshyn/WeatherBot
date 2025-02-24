using WeatherBot.API.Models;

namespace WeatherBot.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByTelegramIdAsync(long telegramId);
        Task<int> AddUserAsync(User user);
        Task<List<User>> GetAllUsersAsync();

    }
}
