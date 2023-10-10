using Sportomondo.Api.Models;

namespace Sportomondo.Api.Responses
{
    public class ActivityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateFinish { get; set; }
        public string Type { get; set; }
        public decimal Distance { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan Pace { get; set; }
        public int AverageHr { get; set; }
        public string City { get; set; }
        public decimal KcalBurned { get; set; }
        public decimal WeatherTemperatureC { get; set; }
        public string WeatherDescription { get; set; }
        public string RouteUrl { get; set; }
        public int UserId { get; set; }
    }
}
