using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Users.Infra.Data.Contexts;

namespace Users.Tests.Fixtures
{
    public abstract class MsSqlIntegrationTestBase : IAsyncLifetime
    {
        protected readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

        protected SqlServerContext SqlServerContext { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();

            var options = new DbContextOptionsBuilder<SqlServerContext>()
                .UseSqlServer(_msSqlContainer.GetConnectionString())
                .Options;

            SqlServerContext = new SqlServerContext(options);

            await SqlServerContext.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await SqlServerContext.DisposeAsync();
            await _msSqlContainer.DisposeAsync();
        }
    }
}