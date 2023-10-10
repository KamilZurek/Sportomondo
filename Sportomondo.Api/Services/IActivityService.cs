using Sportomondo.Api.Models;

namespace Sportomondo.Api.Services
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetAll();
    }
}
