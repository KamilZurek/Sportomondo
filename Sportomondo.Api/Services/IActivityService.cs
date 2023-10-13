using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetAllAsync();
        Task<int> CreateAsync(CreateActivityRequest activity);
        Task<Activity> GetByIdAsync(int id);
    }
}
