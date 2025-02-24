using WeatherBot.API.Models;

namespace WeatherBot.API.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByTelegramIdAsync(long telegramId);
        Task<int> AddUserAsync(User user);
        Task<List<User>> GetAllUsersAsync();
    }
}
