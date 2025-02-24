using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System.Net;
using WeatherBot.API.DTOs;
using WeatherBot.API.Services;
using WeatherBot.API.Services.Interfaces;

namespace WeatherBot.Tests
{
    public class WeatherServiceTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly IWeatherService _weatherService;
        private readonly MockHttpMessageHandler _mockHttp;

        public WeatherServiceTests()
        {
            _configMock = new Mock<IConfiguration>();
            _mockHttp = new MockHttpMessageHandler();

            var httpClient = new HttpClient(_mockHttp);
            _configMock.Setup(x => x["BotConfig:WeatherApiKey"]).Returns("TEST_API_KEY");

            _weatherService = new WeatherService(_configMock.Object, httpClient);
        }

        [Fact]
        public async Task GetWeatherAsync_Should_Return_WeatherResponse_When_Api_Is_Successful()
        {
            // Arrange
            var city = "Kyiv";
            var expectedWeather = new WeatherResponse
            {
                Main = new Main { Temp = 15.3 },
                Weather = new[] { new Weather { Description = "clear sky" } }
            };

            _mockHttp.When($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid=TEST_API_KEY&units=metric")
                     .Respond("application/json", JsonConvert.SerializeObject(expectedWeather));

            // Act
            var result = await _weatherService.GetWeatherAsync(city);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(15.3, result.Main.Temp);
            Assert.Equal("clear sky", result.Weather[0].Description);
        }

        [Fact]
        public async Task GetWeatherAsync_Should_Return_Null_When_Api_Returns_Error()
        {
            // Arrange
            var city = "UnknownCity";

            _mockHttp.When($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid=TEST_API_KEY&units=metric")
                     .Respond(HttpStatusCode.NotFound);

            // Act
            var result = await _weatherService.GetWeatherAsync(city);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetWeatherAsync_Should_Return_Null_When_Response_Is_Empty()
        {
            // Arrange
            var city = "Kyiv";

            _mockHttp.When($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid=TEST_API_KEY&units=metric")
                     .Respond("application/json", "");

            // Act
            var result = await _weatherService.GetWeatherAsync(city);

            // Assert
            Assert.Null(result);
        }
    }
}
