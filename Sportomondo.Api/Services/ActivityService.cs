using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Models;

namespace Sportomondo.Api.Services
{
    public class ActivityService : IActivityService
    {
        private readonly SportomondoDbContext _dbContext;

        public ActivityService(SportomondoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Activity>> GetAll()
        {
            var activities = await _dbContext.Activities
                .Include(a => a.Weather)
                .Include(a => a.User)
                .ToListAsync();

            return activities;
        }
    }
}
