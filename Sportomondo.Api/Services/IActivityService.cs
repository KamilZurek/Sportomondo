using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetAllAsync(string searchPhraseNameCity, CancellationToken cancellationToken = default);
        Task<int> CreateAsync(CreateActivityRequest request, CancellationToken cancellationToken = default);
        Task<Activity> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, ActivityRequest request, CancellationToken cancellationToken = default);
    }
}
