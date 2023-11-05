using Microsoft.AspNetCore.Authorization;

namespace Sportomondo.Api.Authorization
{
    public class AuthorizationRequirement : IAuthorizationRequirement
    {
        public string RolePermission { get; }

        public AuthorizationRequirement(string rolePermission)
        {
            RolePermission = rolePermission;
        }
    }
}
