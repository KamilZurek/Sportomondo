using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportomondo.Api.Authorization;
using Sportomondo.Api.Mapping;
using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;
using Sportomondo.Api.Responses;
using Sportomondo.Api.Services;

namespace Sportomondo.Api.Controllers
{
    [Route("api/activities")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _service;

        public ActivityController(IActivityService service)
        {
            _service = service;
        }
        
        [HttpGet]
        [Authorize(Policy = Policies.ActivityGetAll)]
        public async Task<ActionResult<IEnumerable<ActivityResponse>>> GetAll([FromQuery] string searchPhraseNameCity, CancellationToken cancellationToken)
        {
            var activities = await _service.GetAllAsync(searchPhraseNameCity, cancellationToken);
            var results = activities.Select(a => a.MapToResponse());

            return Ok(results);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Policies.ActivityGet)]
        public async Task<ActionResult<ActivityResponse>> Get([FromRoute] int id, CancellationToken cancellationToken)
        {
            var activity = await _service.GetByIdAsync(id, cancellationToken);
            var result = activity.MapToResponse();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = Policies.ActivityCreate)]
        public async Task<ActionResult> Create([FromBody] CreateActivityRequest request, CancellationToken cancellationToken)
        {
            var createdId = await _service.CreateAsync(request, cancellationToken);

            return Created($"api/activities/{createdId}", null);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.ActivityDelete)]
        public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Policies.ActivityUpdate)]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] ActivityRequest request, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(id, request, cancellationToken);

            return Ok();
        }
    }
}
