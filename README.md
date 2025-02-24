# WeatherBot API

## Overview
WeatherBot is a Telegram bot built using ASP.NET Core Web API that provides weather updates based on user requests. The bot fetches weather data from the OpenWeatherMap API and maintains a history of user queries using an MS SQL database. The API is documented using Swagger for easy testing and interaction.

## Features
- Retrieve weather data for a specified city via Telegram bot commands.
- Store and manage user data along with weather request history using Dapper.
- Send weather updates to all users or a specific user.
- Inline button for requesting weather updates.
- API endpoints for user and weather history management.
- Swagger documentation for testing the API.

## Technologies Used
- **.NET**: ASP.NET Core Web API
- **Database**: MS SQL Server
- **ORM**: Dapper
- **Telegram API**: Telegram.Bot library
- **Weather API**: OpenWeatherMap
- **Swagger**: API documentation and testing

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- MS SQL Server
- Telegram Bot Token (from BotFather)
- OpenWeatherMap API Key

### Configuration
1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo/WeatherBot.git
   cd WeatherBot
   ```
2. Configure `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=WeatherBotDb;Trusted_Connection=True;"
     },
     "BotConfig": {
       "Token": "YOUR_TELEGRAM_BOT_TOKEN"
     },
     "WeatherAPI": {
       "BaseUrl": "https://api.openweathermap.org/data/2.5/weather",
       "ApiKey": "YOUR_OPENWEATHERMAP_API_KEY"
     }
   }
   ```
3. Apply database migrations:
   ```sh
   dotnet ef database update
   ```
4. Run the application:
   ```sh
   dotnet run
   ```

## API Endpoints

### User Management
- `GET /api/users/{telegramId}` - Retrieve user details and query history.
- `POST /api/sendWeatherToAll` - Send weather updates to all users.
- `POST /api/sendWeatherToUser/{userId}` - Send weather updates to a specific user.

### Weather Management
- `GET /api/weather/{city}` - Fetch weather information for a specific city.

## Telegram Bot Commands
- `/start` - Initialize bot interaction.
- `/weather {city}` - Get weather information for a specified city.
- Inline button "🌤 Get Weather" - Prompt user to enter a city.

## Testing
- Unit tests are implemented using **xUnit, Moq**.
  ```sh
  dotnet test
  ```


