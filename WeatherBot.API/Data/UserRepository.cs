using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WeatherBot.API.Data.Interfaces;
using WeatherBot.API.Models;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _db;

    public UserRepository(IConfiguration config)
    {
        _db = new SqlConnection(config.GetConnectionString("DefaultConnection"));
    }

    public async Task<User?> GetUserByTelegramIdAsync(long telegramId)
    {
        var query = @"
        SELECT * FROM Users WHERE TelegramId = @TelegramId;
        SELECT * FROM WeatherHistory WHERE UserId = (SELECT Id FROM Users WHERE TelegramId = @TelegramId) 
        ORDER BY CreatedAt DESC;";

        using var multi = await _db.QueryMultipleAsync(query, new { TelegramId = telegramId });

        var user = await multi.ReadSingleOrDefaultAsync<User>();
        if (user != null)
        {
            user.History = (await multi.ReadAsync<WeatherHistory>()).ToList();
        }
        return user;
    }

    public async Task<int> AddUserAsync(User user)
    {
        var query = @"
        INSERT INTO Users (TelegramId, UserName, CreatedAt)
        OUTPUT INSERTED.Id
        VALUES (@TelegramId, @UserName, @CreatedAt)";

        return await _db.ExecuteScalarAsync<int>(query, user);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return (await _db.QueryAsync<User>("SELECT * FROM Users")).ToList();
    }
}