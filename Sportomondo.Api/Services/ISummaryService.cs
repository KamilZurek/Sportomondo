using Sportomondo.Api.Responses;

namespace Sportomondo.Api.Services
{
    public interface ISummaryService
    {
        Task<SummaryResponse> GetAsync();
    }
}
