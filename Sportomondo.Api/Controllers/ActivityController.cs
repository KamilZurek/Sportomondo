using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<IEnumerable<ActivityResponse>>> GetAll()
        {
            var activities = await _service.GetAllAsync();
            var results = activities.Select(a => a.MapToResponse());

            return Ok(results);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateActivityRequest request)
        {
            var createdId = await _service.CreateAsync(request);

            return Created($"api/activities/{createdId}", null);
        }

        #region later
        //// GET api/<ActivityController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// PUT api/<ActivityController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ActivityController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
        #endregion
    }
}
