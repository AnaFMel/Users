using Microsoft.EntityFrameworkCore;
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
            var user = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
            var password = Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ?? "SenhaAdmin123!";
            var database = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "users_db";
            var host = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";

            var connectionString = $"Server={host};Port=3306;Database={database};Uid={user};Pwd={password}";

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
