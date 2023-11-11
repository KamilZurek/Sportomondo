namespace Sportomondo.Api.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorizationWithPolicies(this IServiceCollection services)
        {
            return services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.ActivityGetAll, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.ActivityGetAll)));
                options.AddPolicy(Policies.ActivityGet, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.ActivityGet)));
                options.AddPolicy(Policies.ActivityCreate, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.ActivityCreate)));
                options.AddPolicy(Policies.ActivityDelete, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.ActivityDelete)));
                options.AddPolicy(Policies.ActivityUpdate, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.ActivityUpdate)));
                options.AddPolicy(Policies.UserChangePassword, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.UserChangePassword)));
                options.AddPolicy(Policies.UserGetAll, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.UserGetAll)));
                options.AddPolicy(Policies.UserDelete, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.UserDelete)));
                options.AddPolicy(Policies.UserChangeRole, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.UserChangeRole)));
                options.AddPolicy(Policies.AchievementGetAll, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.AchievementGetAll)));
                options.AddPolicy(Policies.AchievementCreate, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.AchievementCreate)));
                options.AddPolicy(Policies.AchievementDelete, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.AchievementDelete)));
                options.AddPolicy(Policies.AchievementCheck, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.AchievementCheck)));
            });
        }
    }
}