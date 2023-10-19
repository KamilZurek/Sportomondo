using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public class ActivityService : IActivityService
    {
        private readonly SportomondoDbContext _dbContext;
        private readonly IManageActivityService _manageActivityService;

        public ActivityService(SportomondoDbContext dbContext, IManageActivityService manageActivityService)
        {
            _dbContext = dbContext;
            _manageActivityService = manageActivityService;
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
            var activity = await GetFromDbAsync(id);

            return activity;
        }

        public async Task<int> CreateAsync(CreateActivityRequest request)
        {
            var user = await _dbContext.Users
                .FirstAsync(u => u.Id == request.UserId);
            
            var newActivity = _manageActivityService.CreateFromRequestData(request);

            _manageActivityService.CalculateTime(newActivity);
            _manageActivityService.CalculatePace(newActivity);
            _manageActivityService.CalculateCalories(newActivity, user);
            
            newActivity.Weather = await _manageActivityService.GetWeatherFromAPIAsync(newActivity);

            _dbContext.Activities.Add(newActivity);
            await _dbContext.SaveChangesAsync();

            return newActivity.Id;
        }
        
        public async Task DeleteAsync(int id)
        {
            var activity = await GetFromDbAsync(id);

            _dbContext.Activities.Remove(activity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, ActivityRequest request)
        {
            var activity = await GetFromDbAsync(id);

            var addNewWeather = activity.DateStart.Date != request.DateStart.Date 
                || activity.City.ToUpper() != request.City.ToUpper();

            activity.Name = request.Name;
            activity.DateStart = request.DateStart;
            activity.DateFinish = request.DateFinish;
            activity.Distance = request.Distance;
            activity.AverageHr = request.AverageHr;
            activity.City = request.City;
            activity.RouteUrl = request.RouteUrl;

            _manageActivityService.CalculateTime(activity);
            _manageActivityService.CalculatePace(activity);
            _manageActivityService.CalculateCalories(activity, activity.User);

            if (addNewWeather)
            {
                var currentWeather = activity.Weather;
                if (currentWeather != null)
                {
                    _dbContext.Weathers.Remove(currentWeather);
                }

                var newWeather = await _manageActivityService.GetWeatherFromAPIAsync(activity);

                activity.Weather = newWeather;

                _dbContext.Weathers.Add(newWeather);
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<Activity> GetFromDbAsync(int id)
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
    }
}
