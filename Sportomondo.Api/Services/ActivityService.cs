using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Exceptions;
using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public class ActivityService : IActivityService
    {
        private readonly SportomondoDbContext _dbContext;
        private readonly IManageActivityService _manageActivityService;
        private readonly IHttpContextService _contextService;

        public ActivityService(SportomondoDbContext dbContext, IManageActivityService manageActivityService, IHttpContextService contextService)
        {
            _dbContext = dbContext;
            _manageActivityService = manageActivityService;
            _contextService = contextService;
        }

        /// <summary>
        /// Get all users' activities. Role "Member" can get only its data.
        /// </summary>
        public async Task<IEnumerable<Activity>> GetAllAsync()
        {
            var activities = await _dbContext.Activities
                .Include(a => a.Weather)
                .Include(a => a.User)
                .ToListAsync();

            if (_contextService.UserRoleName == Role.MemberRoleName)
            {
                activities = activities
                    .Where(a => a.UserId == _contextService.UserId)
                    .ToList();
            }
            
            return activities;
        }

        /// <summary>
        /// Get activity by Id. Role "Member" can request only its data.
        /// </summary>
        public async Task<Activity> GetByIdAsync(int id)
        {
            var activity = await GetFromDbAsync(id);

            return activity;
        }

        /// <summary>
        /// Create activity.
        /// </summary>
        public async Task<int> CreateAsync(CreateActivityRequest request)
        {
            var user = await _dbContext.Users
                .FirstAsync(u => u.Id == _contextService.UserId);
            
            var newActivity = _manageActivityService.CreateFromRequestData(request, user.Id);

            _manageActivityService.CalculateTime(newActivity);
            _manageActivityService.CalculatePace(newActivity);
            _manageActivityService.CalculateCalories(newActivity, user);
            
            newActivity.Weather = await _manageActivityService.GetWeatherFromAPIAsync(newActivity);

            _dbContext.Activities.Add(newActivity);
            await _dbContext.SaveChangesAsync();

            return newActivity.Id;
        }

        /// <summary>
        /// Delete activity by Id. Role "Member" can delete only its data.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var activity = await GetFromDbAsync(id);

            _dbContext.Activities.Remove(activity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Update activity by Id. Role "Member" can update only its data.
        /// </summary>
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
                throw new NotFoundException($"There is no activity with id: {id}");
            }

            CheckIfUserRoleCanAccessActivity(activity);

            return activity;
        }

        private void CheckIfUserRoleCanAccessActivity(Activity activity)
        {
            if (_contextService.UserRoleName == Role.MemberRoleName)
            {
                if (activity.UserId != _contextService.UserId)
                {
                    throw new ForbiddenException();
                }
            }

            //other roles can access all activites
        }
    }
}
