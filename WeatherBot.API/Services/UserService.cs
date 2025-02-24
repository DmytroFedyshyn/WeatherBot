using WeatherBot.API.Data.Interfaces;
using WeatherBot.API.Models;
using WeatherBot.API.Services.Interfaces;

namespace WeatherBot.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByTelegramIdAsync(long telegramId)
        {
            return await _userRepository.GetUserByTelegramIdAsync(telegramId);
        }

        public async Task<int> AddUserAsync(User user)
        {
            return await _userRepository.AddUserAsync(user);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }
    }

}
