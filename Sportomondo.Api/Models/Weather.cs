namespace Sportomondo.Api.Models
{
    public class Weather
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public string City { get; set; }
        public decimal TemperatureC { get; set; }
        public string Description { get; set; }
        public int ActivityId { get; set; }
        public Activity Activity { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, {City} {TemperatureC}*C";
        }
    }
}
