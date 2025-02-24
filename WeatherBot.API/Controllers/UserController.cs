using Microsoft.AspNetCore.Mvc;
using WeatherBot.API.Services.Interfaces;

namespace WeatherBot.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{telegramId}")]
        public async Task<IActionResult> GetUser(long telegramId)
        {
            var user = await _userService.GetUserByTelegramIdAsync(telegramId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }
    }
}
