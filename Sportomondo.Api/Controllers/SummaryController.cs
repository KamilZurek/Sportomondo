using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sportomondo.Api.Authorization;
using Sportomondo.Api.Responses;
using Sportomondo.Api.Services;

namespace Sportomondo.Api.Controllers
{
    [Route("api/summaries")]
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private readonly ISummaryService _service;

        public SummaryController(ISummaryService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = Policies.SummaryGet)]
        public async Task<ActionResult<SummaryResponse>> Get(CancellationToken cancellationToken)
        {
            var summary = await _service.GetAsync(cancellationToken);

            return summary;
        }
    }
}
