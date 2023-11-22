namespace Sportomondo.Api.Responses.Helpers
{
    public class ActivityTotals
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal TotalDistance { get; set; }
        public TimeSpan TotalTime { get; set; }
        public decimal AverageHr { get; set; }
        public decimal TotalKcalBurned { get; set; } 
    }
}
