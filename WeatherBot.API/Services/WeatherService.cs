using Newtonsoft.Json;
using WeatherBot.API.DTOs;
using WeatherBot.API.Services.Interfaces;

namespace WeatherBot.API.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(IConfiguration config, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = config["BotConfig:WeatherApiKey"];
        }

        public async Task<WeatherResponse?> GetWeatherAsync(string city)
        {
            var response = await _httpClient.GetAsync(
                $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WeatherResponse>(json);
        }
    }
}
