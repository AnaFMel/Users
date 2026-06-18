using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Users.Infra.CrossCutting.Security;
using Users.Infra.Data.Contexts;
using Users.Infra.Data.Repositories;

namespace Users.Infra.CrossCutting.IoC
{
    public static class ContainerExtensions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MySqlContext>();

            services.AddTransient<JwtService>();
            services.AddTransient<UserService>();
            services.AddTransient<PasswordService>();

            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }
    }
}
