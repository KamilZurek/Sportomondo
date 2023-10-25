﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult> Register([FromBody] RegisterUserRequest request)
        {
            await _userService.RegisterAsync(request);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserRequest request)
        {
            return NoContent();
        }

        [HttpPost("changePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangeUserPasswordRequest request)
        {
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            return NoContent();
        }

        [HttpPost("{id}/changeRole")]
        public async Task<ActionResult> ChangeRole([FromRoute] int id, [FromBody] string newRoleName)
        {
            return NoContent();
        }
    }
}
