using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Users.Domain.Services.Security;
using Users.Infra.Data.Contexts;
using Users.Infra.Data.Repositories;

namespace Users.Infra.CrossCutting.IoC
{
    public static class ContainerExtensions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["MySqlConnectionString"] ?? string.Empty;

            services.AddDbContext<MySqlContext>(options =>
            {
                options.UseMySQL(connectionString, mysqlOptions =>
                {
                    mysqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null
                    );
                });
            });

            services.AddTransient<JwtService>();
            services.AddTransient<UserService>();
            services.AddTransient<PasswordService>();

            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }
    }
}