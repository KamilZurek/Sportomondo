using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetAllAsync(string searchPhraseNameCity, CancellationToken cancellationToken);
        Task<int> CreateAsync(CreateActivityRequest request, CancellationToken cancellationToken);
        Task<Activity> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task UpdateAsync(int id, ActivityRequest request, CancellationToken cancellationToken);
    }
}
