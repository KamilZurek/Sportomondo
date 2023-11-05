using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Sportomondo.Api.Services
{
    public class HttpContextService : IHttpContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User; //fix

        public int UserId => User == null
            ? 0 
            : int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value); //fix

        public string UserRoleName => User?.FindFirst(c => c.Type == ClaimTypes.Role)?.Value; //fix
    }
}
