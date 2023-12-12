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
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        //no authorization policy
        public async Task<ActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            await _service.RegisterAsync(request, cancellationToken);

            return Ok();
        }

        [HttpPost("login")]
        //no authorization policy
        public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            var token = await _service.LoginAsync(request, cancellationToken);

            return Ok(token);
        }

        [HttpPut("changePassword")]
        [Authorize(Policy = Policies.UserChangePassword)]
        public async Task<ActionResult> ChangePassword([FromBody] ChangeUserPasswordRequest request, CancellationToken cancellationToken)
        {
            await _service.ChangePasswordAsync(request, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Authorize(Policy = Policies.UserGetAll)]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var users = await _service.GetAllAsync(cancellationToken);
            var results = users.Select(u => u.MapToResponse());

            return Ok(results);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.UserDelete)]
        public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        [HttpPut("{id}/changeRole")]
        [Authorize(Policy = Policies.UserChangeRole)]
        public async Task<ActionResult> ChangeRole([FromRoute] int id, [FromBody] string newRoleName, CancellationToken cancellationToken)
        {
            await _service.ChangeRoleAsync(id, newRoleName, cancellationToken);

            return Ok();
        }
    }
}
