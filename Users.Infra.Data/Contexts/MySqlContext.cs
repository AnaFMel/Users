using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Users.Domain.Entities;
using Users.Domain.Services;
using Users.Infra.Data.Configurations;

namespace Users.Infra.Data.Contexts
{
    public class MySqlContext : DbContext
    {
        private readonly PasswordService _passwordService;
        private readonly IConfiguration _configuration;

        public MySqlContext(PasswordService senhaService, IConfiguration configuration, DbContextOptions<MySqlContext> options) : base(options)
        {
            _passwordService = senhaService;
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var user = Environment.GetEnvironmentVariable("MARIADB_USER") ?? "root";
                var password = Environment.GetEnvironmentVariable("MARIADB_ROOT_PASSWORD") ?? "SenhaAdmin123!";
                var database = Environment.GetEnvironmentVariable("MARIADB_DATABASE") ?? "users_db";
                var host = Environment.GetEnvironmentVariable("MARIADB_HOST") ?? "localhost";
                var port = Environment.GetEnvironmentVariable("MARIADB_HOST") is null ? "3307" : "3306";

                var connectionString = $"Server={host};Port={port};Database={database};Uid={user};Pwd={password}";

                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    optionsBuilder.UseMySQL(connectionString);
                }
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration(_passwordService));

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
