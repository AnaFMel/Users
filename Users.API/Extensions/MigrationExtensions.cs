using Microsoft.EntityFrameworkCore;
using Users.Infra.Data.Contexts;

namespace Users.API.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MySqlContext>();

            if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();
        }
    }
}