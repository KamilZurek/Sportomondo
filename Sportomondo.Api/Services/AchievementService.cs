﻿using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly SportomondoDbContext _dbContext;
        private readonly IHttpContextService _contextService;

        public AchievementService(SportomondoDbContext dbContext, IHttpContextService contextService)
        {
            _dbContext = dbContext;
            _contextService = contextService;
        }

        public async Task<IEnumerable<Achievement>> GetAllAsync(bool onlyMine)
        {
            var achievements = await _dbContext.Achievements
                .Include(a => a.Users)
                .ToListAsync();

            if (onlyMine)
            {
                var user = await _dbContext.Users
                    .FirstAsync(u => u.Id == _contextService.UserId);

                achievements = achievements
                                    .Where(a => a.Users.Contains(user))
                                    .ToList();
            }

            return achievements;
        }

        public async Task<int> CreateAsync(CreateAchievementRequest request)
        {
            var achievement = new Achievement()
            {
                Name = request.Name,
                ActivityType = request.ActivityType,
                CountingType = request.CountingType,
                CountingRequiredValue = request.CountingRequiredValue,
                Points = request.Points
            };

            _dbContext.Achievements.Add(achievement);
            await _dbContext.SaveChangesAsync();

            return achievement.Id;
        }

        public async Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CheckAsync()
        {
            var achievementsInfo = string.Empty;
            
            var achievements = await _dbContext.Achievements
                .Include(a => a.Users)
                .ToListAsync();

            var users = await _dbContext.Users
                .Include(u => u.Activities)
                .ToListAsync();

            if (_contextService.UserRoleName == Role.MemberRoleName) //Member checks only its achievements
            {
                achievementsInfo = CheckAchievementsForUser(achievements, users.First(u => u.Id == _contextService.UserId));
            }
            else //Admin & Support check all users' achievements
            {
                foreach (var user in users)
                {
                    achievementsInfo += CheckAchievementsForUser(achievements, user);
                }
            }

            await _dbContext.SaveChangesAsync();

            return achievementsInfo;
        }

        private string CheckAchievementsForUser(List<Achievement> achievements, User user)
        {
            string result = string.Empty;

            var achievementsBeforeCount = achievements.Count(a => a.Users.Contains(user));
            result += $"User: {user.Id} had {achievementsBeforeCount} achievements before 'Check' operation\n";

            foreach (var achievement in achievements)
            {
                var shouldAssign = ShouldAssignAchievementToUser(achievement, user);

                if (shouldAssign)
                {
                    if (!achievement.Users.Contains(user))
                    {
                        achievement.Users.Add(user);

                        result += $"User: {user.Id} gained achievement '{achievement.Name}'\n";
                    }
                }
                else
                {
                    if (achievement.Users.Contains(user))
                    {
                        achievement.Users.Remove(user);

                        result += $"User: {user.Id} lost achievement '{achievement.Name}'\n";
                    }
                }
            }

            var achievementsAfterCount = achievements.Count(a => a.Users.Contains(user));
            result += $"User: {user.Id} has {achievementsAfterCount} achievements after 'Check' operation\n";

            return result;
        }

        private bool ShouldAssignAchievementToUser(Achievement achievement, User user)
        {
            var userActivitiesByType = user.Activities
                    .Where(a => a.Type == achievement.ActivityType);

            if (achievement.CountingType == CountingType.Time)
            {
                var totalTime = TimeSpan.Zero;

                foreach (var activity in userActivitiesByType)
                {
                    totalTime += activity.Time;
                }

                var requiredTime = new TimeSpan((int)achievement.CountingRequiredValue, 0, 0);

                return totalTime >= requiredTime;
            }
            else if (achievement.CountingType == CountingType.Distance)
            {
                var totalDistance = 0m;

                foreach (var activity in userActivitiesByType)
                {
                    totalDistance += activity.Distance;
                }

                var requiredDistance = achievement.CountingRequiredValue;

                return totalDistance >= requiredDistance;
            }
            else
            {
                throw new Exception("Unhandled CountingType");
            }
        }
    }
}