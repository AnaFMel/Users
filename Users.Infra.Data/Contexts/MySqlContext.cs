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
                var connectionString = _configuration.GetConnectionString(nameof(Contexts.Database.MySql));

                if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException($"The connection string for the database {nameof(Contexts.Database.MySql)} was not found.");

                optionsBuilder.UseMySQL(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration(_passwordService));
        }
    }
}
