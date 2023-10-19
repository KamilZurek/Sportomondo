using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetAllAsync();
        Task<int> CreateAsync(CreateActivityRequest request);
        Task<Activity> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task UpdateAsync(int id, ActivityRequest request);
    }
}
