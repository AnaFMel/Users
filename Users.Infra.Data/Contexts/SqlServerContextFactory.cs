using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Users.Infra.Data.Contexts
{
    public class SqlServerContextFactory : IDesignTimeDbContextFactory<SqlServerContext>
    {
        public SqlServerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerContext>();

            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=fcg;Trusted_Connection=True;TrustServerCertificate=True;");

            return new SqlServerContext(optionsBuilder.Options);
        }
    }
}