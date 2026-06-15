using Users.Domain.Enums;

namespace Users.API.Configurations
{
    public static class PolicyExtensions
    {
        public static IServiceCollection AddPolicies(this IServiceCollection services)
        {
            services.AddAuthorizationBuilder()
                .AddPolicy(nameof(Policy.Admin), policy => policy.RequireRole(nameof(Policy.Admin)))
                .AddPolicy(nameof(Policy.User), policy => policy.RequireRole(nameof(Policy.User)))
                .AddPolicy(nameof(Policy.All), policy => policy.RequireRole(nameof(Policy.Admin), nameof(Policy.User)));

            return services;
        }
    }
}
