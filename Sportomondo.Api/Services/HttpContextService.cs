using Microsoft.AspNetCore.Http;
using Sportomondo.Api.Exceptions;
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

        public ClaimsPrincipal User
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                
                if (user == null)
                {
                    throw new InvalidTokenException();
                }

                return user;
            }
        }

        public int UserId
        {
            get
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

                    return userId;
                }
                catch
                {
                    throw new InvalidTokenException();
                }
            }
        }

        public string UserRoleName
        {
            get
            {
                try
                {
                    var userRoleName = User.FindFirst(c => c.Type == ClaimTypes.Role).Value;

                    return userRoleName;
                }
                catch
                {
                    throw new InvalidTokenException();
                }
            }
        }
    }
}
