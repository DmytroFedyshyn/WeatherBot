using System.Data;
using Microsoft.Data.SqlClient;
using Telegram.Bot;
using WeatherBot.API.Services.Interfaces;
using WeatherBot.API.Services;
using WeatherBot.API.Data.Interfaces;
using WeatherBot.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITelegramBotClient>(sp =>
    new TelegramBotClient(builder.Configuration["BotConfig:Token"]));

builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddSingleton<ITelegramBotService, TelegramBotService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWeatherHistoryRepository, WeatherHistoryRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddHttpClient();

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var botService = scope.ServiceProvider.GetRequiredService<ITelegramBotService>();
    await botService.StartAsync();
}

app.Run();
