using Microsoft.EntityFrameworkCore;
using QuickChart;
using Sportomondo.Api.Context;
using Sportomondo.Api.Helpers;
using Sportomondo.Api.Models;
using Sportomondo.Api.Responses;
using Sportomondo.Api.Responses.Helpers;
using System.Globalization;
using System.Text.Json;

namespace Sportomondo.Api.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly SportomondoDbContext _dbContext;
        private readonly IHttpContextService _contextService;

        public SummaryService(SportomondoDbContext dbContext, IHttpContextService contextService)
        {
            _dbContext = dbContext;
            _contextService = contextService;
        }

        /// <summary>
        /// Get summary for current user
        /// </summary>
        public async Task<SummaryResponse> GetAsync(CancellationToken cancellationToken)
        {
            var user = await GetUserAsync(cancellationToken);

            var chartUrl = GetActivitiesChartUrl(user);
            var activitesTotals = GetActivitiesTotals(user);

            return new SummaryResponse()
            {
                User = user.ToString(),
                ActivitiesChartUrl = chartUrl,
                ActivitiesCount = user.Activities.Count,
                AchievementsCount = user.Achievements.Count,
                AchievementsPoints = user.Achievements.Sum(x => x.Points),
                ActivitiesTotals = activitesTotals
            };
        }

        private async Task<User> GetUserAsync(CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(u => u.Activities)
                .Include(u => u.Achievements)
                .FirstAsync(u => u.Id == _contextService.UserId, cancellationToken);

            return user;
        }

        private string GetActivitiesChartUrl(User user)
        {
            var config = GetActivitiesChartConfig(user);
            var configJson = JsonSerializer.Serialize(config);

            var chart = new Chart
            {
                Width = 800,
                Height = 300,
                Version = "2.9.4",
                Config = configJson
            };

            return chart.GetShortUrl();
        }

        private ActivitiesChartConfig GetActivitiesChartConfig(User user)
        {
            return new ActivitiesChartConfig()
            {
                Type = "bar",
                Data = GetActivitiesChartConfigData(user)
            };
        }

        private ActivitiesChartConfigData GetActivitiesChartConfigData(User user)
        {
            var months = new CultureInfo("en-US").DateTimeFormat.MonthNames
                    .Take(12)
                    .ToArray();

            return new ActivitiesChartConfigData()
            {
                Labels = months,
                Datasets = GetActivitiesChartConfigDataDatasets(user, months)
            };
        }

        private ActivitiesChartConfigDataDataset[] GetActivitiesChartConfigDataDatasets(User user, string[] months)
        {
            var activityTypes = Enum.GetValues(typeof(ActivityType));

            var datasets = new ActivitiesChartConfigDataDataset[activityTypes.Length];

            int dataSetsCounter = 0;

            foreach (ActivityType type in activityTypes)
            {
                var activitiesByType = user.Activities
                    .Where(a => a.Type == type);

                var dataset = new ActivitiesChartConfigDataDataset()
                {
                    Label = type.ToString(),
                    Data = new int[months.Length]
                };

                int monthCounter = 0;

                foreach (var month in months)
                {
                    var currentMonth = monthCounter + 1;
                    var activitiesInMonth = activitiesByType.Where(a => a.DateStart.Year == DateTime.Now.Year 
                                                                        && a.DateStart.Month == currentMonth);

                    var distanceInMonth = activitiesInMonth.Sum(a => a.Distance);
                    dataset.Data[monthCounter] = (int)Math.Round(distanceInMonth);

                    monthCounter++;
                }

                datasets[dataSetsCounter] = dataset;

                dataSetsCounter++;
            }

            return datasets;
        }

        private List<ActivityTotals> GetActivitiesTotals(User user)
        {
            var activitiestotals = new List<ActivityTotals>();

            foreach (ActivityType type in Enum.GetValues(typeof(ActivityType)))
            {
                var activitiesByType = user.Activities
                    .Where(a => a.Type == type);

                activitiestotals.Add(new ActivityTotals()
                {
                    Name = type.ToString(),
                    Count = activitiesByType.Count(),
                    TotalDistance = activitiesByType.Sum(a => a.Distance),
                    TotalTime = GetActivityTypeTotalTime(activitiesByType),
                    AverageHr = Math.Round(activitiesByType.Average(a => (decimal)a.AverageHr)),
                    TotalKcalBurned = activitiesByType.Sum(a => a.KcalBurned)
                });
            }

            return activitiestotals;
        }

        private TimeSpan GetActivityTypeTotalTime(IEnumerable<Activity> activitiesByType)
        {
            TimeSpan totalTime = TimeSpan.Zero;

            foreach (var activity in activitiesByType)
            {
                totalTime = totalTime.Add(activity.Time);
            }

            return totalTime;
        }
    }
}
