using Sportomondo.Api.Models;
using Sportomondo.Api.Responses;

namespace Sportomondo.Api.Mapping
{
    public static class ContractMapping
    {
        public static ActivityResponse MapToResponse(this Activity activity)
        {
            return new ActivityResponse()
            {
                Id = activity.Id,
                Name = activity.Name,
                DateStart = activity.DateStart,
                DateFinish = activity.DateFinish,
                Type = activity.Type.ToString(),
                Distance = activity.Distance,
                Time = activity.Time,
                Pace = activity.Pace,
                AverageHr = activity.AverageHr,
                City = activity.City,
                KcalBurned = activity.KcalBurned,
                WeatherTemperatureC = activity.Weather.TemperatureC,
                WeatherDescription = activity.Weather.Description,
                RouteUrl = activity.RouteUrl,
                UserId = activity.UserId
            };
        }

        public static UserResponse MapToResponse(this User user)
        {
            return new UserResponse()
            {
                Id = user.Id,
                Name = user.FirstName + " " + user.LastName,
                DateOfBirth = user.DateOfBirth,
                Weight = user.Weight,
                Role = user.Role.Name,
                ActivitiesCount = user.Activities.Count,
                AchievementsPoints = user.Achievements.Sum(x => x.Points)
            };
        }

        public static AchievementResponse MapToResponse(this Achievement achievement)
        {
            return new AchievementResponse()
            {
                Id = achievement.Id,
                Name = achievement.Name,
                ActivityType = achievement.ActivityType.ToString(),
                CountingType = achievement.CountingType.ToString(),
                CountingRequiredValue = achievement.CountingRequiredValue,
                Points = achievement.Points,
                WhoHas = achievement.Users.Select(x => x.FirstName + " " + x.LastName).ToList()
            };
        }
    }
}
