using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Exceptions;
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

        /// <summary>
        /// Get all achievements
        /// </summary>
        public async Task<IEnumerable<Achievement>> GetAllAsync(bool onlyMine, CancellationToken cancellationToken = default)
        {
            var achievements = await _dbContext.Achievements
                .Include(a => a.Users)
                .ToListAsync(cancellationToken);

            if (onlyMine)
            {
                var user = await _dbContext.Users
                    .FirstAsync(u => u.Id == _contextService.UserId, cancellationToken);

                achievements = achievements
                                    .Where(a => a.Users.Contains(user))
                                    .ToList();
            }

            return achievements;
        }

        /// <summary>
        /// Create achievement
        /// </summary>
        public async Task<int> CreateAsync(CreateAchievementRequest request, CancellationToken cancellationToken = default)
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
            await _dbContext.SaveChangesAsync(cancellationToken);

            return achievement.Id;
        }

        /// <summary>
        /// Delete achievement by Id
        /// </summary>
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var achievement = await _dbContext.Achievements
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (achievement == null)
            {
                throw new NotFoundException($"There is no achievement with id: {id}");
            }

            _dbContext.Achievements.Remove(achievement);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Check and assign achievements to users
        /// </summary>
        public async Task<string> CheckAsync(CancellationToken cancellationToken = default)
        {
            var achievementsInfo = string.Empty;
            
            var achievements = await _dbContext.Achievements
                .Include(a => a.Users)
                .ToListAsync(cancellationToken);

            var users = await _dbContext.Users
                .Include(u => u.Activities)
                .ToListAsync(cancellationToken);

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

            await _dbContext.SaveChangesAsync(cancellationToken);

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
