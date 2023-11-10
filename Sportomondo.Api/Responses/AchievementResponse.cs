using Sportomondo.Api.Models;

namespace Sportomondo.Api.Responses
{
    public class AchievementResponse
    {
        public int Id { get; set; }
        public  string Name { get; set; }
        public string ActivityType { get; set; }
        public string CountingType { get; set; }
        public decimal CountingRequiredValue { get; set; }
        public int Points { get; set; }
        public List<string> WhoHas { get; set; }
    }
}
