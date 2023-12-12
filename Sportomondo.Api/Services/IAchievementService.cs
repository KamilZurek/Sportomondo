using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public interface IAchievementService
    {
        Task<IEnumerable<Achievement>> GetAllAsync(bool onlyMine, CancellationToken cancellationToken);
        Task<int> CreateAsync(CreateAchievementRequest request, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<string> CheckAsync(CancellationToken cancellationToken);
    }
}
