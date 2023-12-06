using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Sportomondo.Api.Context;
using Sportomondo.Api.Models;

namespace Sportomondo.Tests.Helpers
{
    public static class Utilities
    {
        public static async Task InitializeActivitiesForTests(SportomondoDbContext dbContext)
        {
            if (dbContext.Activities.Any())
            {
                dbContext.Activities.RemoveRange(dbContext.Activities);
            }

            dbContext.Activities.AddRange(GetSeedingActivities());
            await dbContext.SaveChangesAsync();
        }

        private static List<Activity> GetSeedingActivities()
        {
            return new List<Activity>()
            {
                new Activity()
                {
                    Name = "Test Running",
                    DateStart = DateTime.Now,
                    DateFinish = DateTime.Now.AddHours(1),
                    Type = 0,
                    Distance = 10,
                    Time = new TimeSpan(1, 0, 0),
                    Pace = new TimeSpan(0, 6, 0),
                    AverageHr = 150,
                    City = "Cracow",
                    KcalBurned = 500,
                    Weather = new Weather()
                    {
                        Day = DateTime.Now,
                        City = "Cracow",
                        TemperatureC = 30,
                        Description = "Test Weather"
                    },
                    RouteUrl = "empty",
                    UserId = 1
                },
                new Activity()
                {
                    Name = "Test Running 2",
                    DateStart = DateTime.Now,
                    DateFinish = DateTime.Now.AddHours(2),
                    Type = 0,
                    Distance = 20,
                    Time = new TimeSpan(2, 0, 0),
                    Pace = new TimeSpan(0, 6, 0),
                    AverageHr = 180,
                    City = "Warsaw",
                    KcalBurned = 1000,
                    Weather = new Weather()
                    {
                        Day = DateTime.Now,
                        City = "Warsaw",
                        TemperatureC = 35,
                        Description = "Test Weather 2"
                    },
                    RouteUrl = "empty 2",
                    UserId = 1
                }
            };
        }
    }
}
