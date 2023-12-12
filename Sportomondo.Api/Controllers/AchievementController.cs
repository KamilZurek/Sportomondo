using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sportomondo.Api.Authorization;
using Sportomondo.Api.Mapping;
using Sportomondo.Api.Requests;
using Sportomondo.Api.Responses;
using Sportomondo.Api.Services;

namespace Sportomondo.Api.Controllers
{
    [Route("api/achievements")]
    [ApiController]
    public class AchievementController : ControllerBase
    {
        private readonly IAchievementService _service;

        public AchievementController(IAchievementService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = Policies.AchievementGetAll)]
        public async Task<ActionResult<IEnumerable<AchievementResponse>>> GetAll([FromQuery] bool onlyMine, CancellationToken cancellationToken)
        {
            var achievements = await _service.GetAllAsync(onlyMine, cancellationToken);
            var results = achievements.Select(a => a.MapToResponse());

            return Ok(results);
        }

        [HttpPost]
        [Authorize(Policy = Policies.AchievementCreate)]
        public async Task<ActionResult> Create([FromBody] CreateAchievementRequest request, CancellationToken cancellationToken)
        {
            var createdId = await _service.CreateAsync(request, cancellationToken);

            return Created($"api/achievements/{createdId}", null);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.AchievementDelete)]
        public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        [HttpPut("check")]
        [Authorize(Policy = Policies.AchievementCheck)]
        public async Task<ActionResult<string>> Check(CancellationToken cancellationToken)
        {
            var result = await _service.CheckAsync(cancellationToken);

            return Ok(result);
        }
    }
}
