using Dapper;
using System.Data;
using WeatherBot.API.Data.Interfaces;
using WeatherBot.API.Models;

namespace WeatherBot.API.Data
{
    public class WeatherHistoryRepository : IWeatherHistoryRepository
    {
        private readonly IDbConnection _db;

        public WeatherHistoryRepository(IDbConnection db) => _db = db;

        public async Task<int> AddWeatherHistoryAsync(WeatherHistory history)
        {
            var query = @"
                        INSERT INTO WeatherHistory (UserId, City, Temperature, Description, CreatedAt)
                        VALUES (@UserId, @City, @Temperature, @Description, @CreatedAt)";

            return await _db.ExecuteAsync(query, history);
        }
    }
}
