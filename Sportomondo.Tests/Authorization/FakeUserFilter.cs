using Microsoft.AspNetCore.Mvc.Filters;
using Sportomondo.Api.Models;
using System.Security.Claims;

namespace Sportomondo.Tests.Authorization
{
    public class FakeUserFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimsPrincipal = new ClaimsPrincipal();

            claimsPrincipal.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@sportomondo.pl"),
                new Claim(ClaimTypes.Name, "Test Tested"),
                new Claim(ClaimTypes.Role, Role.AdminRoleName)
            }));

            context.HttpContext.User = claimsPrincipal;

            await next();
        }
    }
}
