using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [Authorize(Policy = "")]
        public async Task<ActionResult<IEnumerable<AchievementResponse>>> GetAll()
        {
            var achievements = await _service.GetAllAsync();
            var results = achievements.Select(a => a.MapToResponse());

            return Ok(results);
        }

        [HttpPost]
        [Authorize(Policy = "")]
        public async Task<ActionResult> Create([FromBody] CreateAchievementRequest request)
        {
            var createdId = await _service.CreateAsync(request);

            return Created($"api/achievements/{createdId}", null);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }

        [HttpPut("check")]
        [Authorize(Policy = "")]
        public async Task<ActionResult<string>> Check()
        {
            var result = await _service.CheckAsync();

            return Ok(result);
        }
    }
}
