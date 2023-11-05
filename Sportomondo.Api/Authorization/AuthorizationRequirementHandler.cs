using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Exceptions;
using Sportomondo.Api.Services;

namespace Sportomondo.Api.Authorization
{
    public class AuthorizationRequirementHandler : AuthorizationHandler<AuthorizationRequirement>
    {
        private readonly SportomondoDbContext _dbContext;
        private readonly IHttpContextService _httpContextService;

        public AuthorizationRequirementHandler(SportomondoDbContext dbContext, IHttpContextService httpContextService)
        {
            _dbContext = dbContext;
            _httpContextService = httpContextService;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
        {
            var roleName = _httpContextService.UserRoleName;
            var role = await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
            {
                throw new NotFoundException($"Authorization failed. There is no role in system with Name: {roleName}");
            }

            var rolePermission = await _dbContext.RolePermissions
                .Where(r => r.Name == requirement.RolePermission && r.RoleId == role.Id)
                .ToListAsync();

            if (rolePermission.Count != 1)
            {
                throw new NotFoundException($"Authorization failed. There is zero or more than one role permission " +
                    $"in system with Name: {requirement.RolePermission} for RoleId: {role.Id}");
            }

            if (rolePermission.First().Enabled)
            {
                context.Succeed(requirement);
            }
        }
    }
}
