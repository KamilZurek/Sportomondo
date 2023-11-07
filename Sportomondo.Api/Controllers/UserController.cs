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
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        //no authorization policy
        public async Task<ActionResult> Register([FromBody] RegisterUserRequest request)
        {
            await _userService.RegisterAsync(request);

            return Ok();
        }

        [HttpPost("login")]
        //no authorization policy
        public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserRequest request)
        {
            var token = await _userService.LoginAsync(request);

            return Ok(token);
        }

        [HttpPut("changePassword")]
        [Authorize(Policy = Policies.UserChangePassword)]
        public async Task<ActionResult> ChangePassword([FromBody] ChangeUserPasswordRequest request)
        {
            await _userService.ChangePasswordAsync(request);

            return Ok();
        }

        [HttpGet]
        [Authorize(Policy = Policies.UserGetAll)]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            var results = users.Select(u => u.MapToResponse());

            return Ok(results);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.UserDelete)]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _userService.DeleteAsync(id);

            return NoContent();
        }

        [HttpPut("{id}/changeRole")]
        [Authorize(Policy = Policies.UserChangeRole)]
        public async Task<ActionResult> ChangeRole([FromRoute] int id, [FromBody] string newRoleName)
        {
            await _userService.ChangeRoleAsync(id, newRoleName);

            return Ok();
        }
    }
}
