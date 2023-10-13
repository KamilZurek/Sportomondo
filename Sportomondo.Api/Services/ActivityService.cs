using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public class ActivityService : IActivityService
    {
        private readonly SportomondoDbContext _dbContext;
        private readonly ICreateActivityService _createService;

        public ActivityService(SportomondoDbContext dbContext, ICreateActivityService createService)
        {
            _dbContext = dbContext;
            _createService = createService;
        }

        public async Task<IEnumerable<Activity>> GetAllAsync()
        {
            var activities = await _dbContext.Activities
                .Include(a => a.Weather)
                .Include(a => a.User)
                .ToListAsync();

            return activities;
        }

        public async Task<Activity> GetByIdAsync(int id)
        {
            var activity = await _dbContext.Activities
                .Include(a => a.Weather)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
            {
                throw new Exception($"There is no activity with id: {id}");
            }

            return activity;
        }

        public async Task<int> CreateAsync(CreateActivityRequest request)
        {
            var user = await _dbContext.Users
                .FirstAsync(u => u.Id == request.UserId);
            
            var newActivity = _createService.CreateFromRequestData(request);

            _createService.CalculateTime(newActivity);
            _createService.CalculatePace(newActivity);
            _createService.CalculateCalories(newActivity, user);
            
            newActivity.Weather = await _createService.GetWeatherFromAPIAsync(newActivity);

            _dbContext.Activities.Add(newActivity);
            await _dbContext.SaveChangesAsync();

            return newActivity.Id;
        } 
    }
}
