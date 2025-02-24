using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using WeatherBot.API.Services.Interfaces;
using System.Collections.Concurrent;
using Telegram.Bot.Types.ReplyMarkups;
using WeatherBot.API.Data.Interfaces;
using WeatherBot.API.Models;
using User = WeatherBot.API.Models.User;

namespace WeatherBot.API.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IWeatherService _weatherService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConcurrentDictionary<long, bool> _waitingForCity = new();

        public TelegramBotService(ITelegramBotClient botClient, IWeatherService weatherService, IServiceScopeFactory scopeFactory)
        {
            _botClient = botClient;
            _weatherService = weatherService;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync()
        {
            _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                var message = update.Message;
                var chatId = message.Chat.Id;
                var messageText = message.Text.Trim();

                if (_waitingForCity.TryGetValue(chatId, out bool isWaiting) && isWaiting)
                {
                    _waitingForCity.TryRemove(chatId, out _);
                    await SendWeatherInfo(botClient, chatId, messageText, cancellationToken);
                    return;
                }

                if (messageText == "/start")
                {
                    await SendInlineMenu(chatId, cancellationToken);
                    return;
                }

                if (messageText.StartsWith("/weather", StringComparison.OrdinalIgnoreCase))
                {
                    var city = messageText.Substring(8).Trim();

                    if (string.IsNullOrEmpty(city))
                    {
                        _waitingForCity[chatId] = true;
                        await botClient.SendMessage(chatId,
                            "🌍 Please enter the city name.",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    await SendWeatherInfo(botClient, chatId, city, cancellationToken);
                }
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(update.CallbackQuery, cancellationToken);
            }
        }

        private async Task HandleCallbackQuery(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var data = callbackQuery.Data;

            if (data == "get_weather")
            {
                _waitingForCity[chatId] = true;
                await _botClient.SendMessage(chatId, "Please enter the city name.", cancellationToken: cancellationToken);
            }
        }

        public async Task SendInlineMenu(long chatId, CancellationToken cancellationToken = default)
        {
            var keyboard = new InlineKeyboardMarkup(
            [
                [InlineKeyboardButton.WithCallbackData("🌤 Get Weather", "get_weather")]
            ]);

            await _botClient.SendMessage(chatId, "Choose an option:", replyMarkup: keyboard, cancellationToken: cancellationToken);
        }

        private async Task SendWeatherInfo(ITelegramBotClient botClient, long chatId, string city, CancellationToken cancellationToken)
        {
            var weather = await _weatherService.GetWeatherAsync(city);
            if (weather == null)
            {
                await botClient.SendMessage(chatId,
                    "Could not retrieve weather data.",
                    cancellationToken: cancellationToken);
                await SendInlineMenu(chatId, cancellationToken);
                return;
            }

            await SaveWeatherHistory(chatId, city, weather.Main.Temp, weather.Weather[0].Description);
            await SendWeatherMessage(chatId, city, weather.Main.Temp, weather.Weather[0].Description, cancellationToken);
            await SendInlineMenu(chatId, cancellationToken);
        }

        public async Task SendWeatherMessage(long chatId, string city, double temp, string description, CancellationToken cancellationToken = default)
        {
            await _botClient.SendMessage(chatId,
                $"🌤 Weather in {city}:\n🌡 Temperature: {Math.Round(temp)}°C\n📄 {description}",
                cancellationToken: cancellationToken);
        }

        private async Task SaveWeatherHistory(long chatId, string city, double temp, string description)
        {
            using var scope = _scopeFactory.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var weatherHistoryRepository = scope.ServiceProvider.GetRequiredService<IWeatherHistoryRepository>();

            var user = await userRepository.GetUserByTelegramIdAsync(chatId);

            if (user == null)
            {
                user = new User
                {
                    TelegramId = chatId,
                    UserName = $"User_{chatId}",
                    CreatedAt = DateTime.UtcNow
                };

                user.Id = await userRepository.AddUserAsync(user);
                user = await userRepository.GetUserByTelegramIdAsync(chatId);
            }

            if (user == null) return;

            await weatherHistoryRepository.AddWeatherHistoryAsync(new WeatherHistory
            {
                UserId = user.Id,
                City = city,
                Temperature = temp,
                Description = description,
                CreatedAt = DateTime.UtcNow
            });
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
