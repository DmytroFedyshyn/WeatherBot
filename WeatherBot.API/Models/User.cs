namespace WeatherBot.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<WeatherHistory>? History { get; set; }
    }
}
