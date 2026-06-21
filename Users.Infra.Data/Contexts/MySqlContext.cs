using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Services;
using Users.Infra.Data.Configurations;

namespace Users.Infra.Data.Contexts
{
    public class MySqlContext : DbContext
    {
        private readonly PasswordService _passwordService;

        public MySqlContext(PasswordService senhaService, DbContextOptions<MySqlContext> options) : base(options)
        {
            _passwordService = senhaService;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration(_passwordService));
        }
    }
}
