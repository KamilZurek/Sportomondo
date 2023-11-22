using Sportomondo.Api.Responses.Helpers;

namespace Sportomondo.Api.Responses
{
    public class SummaryResponse
    {
        public string User { get; set; }
        public string ActivitiesChartUrl { get; set; }
        public int ActivitiesCount { get; set; }
        public int AchievementsCount { get; set; }
        public int AchievementsPoints { get; set; }
        public List<ActivityTotals> ActivitiesTotals { get; set; }
    }
}
