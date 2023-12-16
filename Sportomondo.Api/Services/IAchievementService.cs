using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public interface IAchievementService
    {
        Task<IEnumerable<Achievement>> GetAllAsync(bool onlyMine, CancellationToken cancellationToken = default);
        Task<int> CreateAsync(CreateAchievementRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<string> CheckAsync(CancellationToken cancellationToken = default);
    }
}
