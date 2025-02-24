using Microsoft.AspNetCore.Mvc;
using WeatherBot.API.Services.Interfaces;

namespace WeatherBot.API.Controllers
{
    [Route("api/weather")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWeatherService _weatherService;
        private readonly ITelegramBotService _botService;

        public WeatherController(IUserService userService, IWeatherService weatherService, ITelegramBotService botService)
        {
            _userService = userService;
            _weatherService = weatherService;
            _botService = botService;
        }

        [HttpPost("sendWeatherToAll")]
        public async Task<IActionResult> SendWeatherToAll([FromBody] string city)
        {
            var users = await _userService.GetAllUsersAsync();
            if (users == null || !users.Any()) return NotFound("No users found.");

            var weather = await _weatherService.GetWeatherAsync(city);
            if (weather == null) return BadRequest("Could not retrieve weather data.");

            foreach (var user in users)
            {
                await _botService.SendWeatherMessage(user.TelegramId, city, weather.Main.Temp, weather.Weather[0].Description);
            }

            return Ok("Weather sent to all users.");
        }

        [HttpPost("sendWeatherToUser/{telegramId}")]
        public async Task<IActionResult> SendWeatherToUser(long telegramId, [FromBody] string city)
        {
            var user = await _userService.GetUserByTelegramIdAsync(telegramId);
            if (user == null) return NotFound("User not found.");

            var weather = await _weatherService.GetWeatherAsync(city);
            if (weather == null) return BadRequest("Could not retrieve weather data.");

            await _botService.SendWeatherMessage(user.TelegramId, city, weather.Main.Temp, weather.Weather[0].Description);

            return Ok($"Weather sent to user {user.UserName}.");
        }
    }

}
