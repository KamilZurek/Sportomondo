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

        public async Task<IEnumerable<Achievement>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateAsync(CreateAchievementRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CheckAsync()
        {
            throw new NotImplementedException();
        }
    }
}
