namespace Sportomondo.Api.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorizationWithPolicies(this IServiceCollection services)
        {
            return services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.User_GetAll, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.User_GetAll)));

                options.AddPolicy(Policies.User_Delete, builder => builder.AddRequirements(new AuthorizationRequirement(Policies.User_Delete)));

                //dodać pozostałe metody
            });
        }
    }
}
