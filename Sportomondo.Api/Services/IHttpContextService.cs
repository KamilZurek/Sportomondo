using System.Security.Claims;

namespace Sportomondo.Api.Services
{
    public interface IHttpContextService
    {
        public ClaimsPrincipal User { get; }
        public int UserId { get; }
        public string UserRoleName { get; }
    }
}
