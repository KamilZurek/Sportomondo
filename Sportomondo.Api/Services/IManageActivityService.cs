using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public interface IManageActivityService
    {
        Activity CreateFromRequestData(CreateActivityRequest request, int userId);
        void CalculateTime(Activity activity);
        void CalculatePace(Activity activity);
        void CalculateCalories(Activity activity, User user);
        Task<Weather> GetWeatherFromAPIAsync(Activity activity);
    }
}
