namespace WeatherBot.API.Models
{
    public class WeatherHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string City { get; set; }
        public double Temperature { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
