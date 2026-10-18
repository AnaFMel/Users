using FluentAssertions;
using Users.Domain.Entities;
using Users.Infra.Data.Repositories;
using Users.Tests.Fixtures;

namespace Users.Tests.Api.Repositories
{
    public class UserRepositoryIntegrationTests : MsSqlIntegrationTestBase
    {
        private UserRepository UserRepository => new(SqlServerContext);

        [Fact]
        public async Task AddAsync_And_GetAsync_ById_ReturnsUser()
        {
            var shortId = Guid.NewGuid().ToString()[..6];
            var email = $"t_{shortId}@fiap.com";
            var user = new User("Teste User", email, "Senha123!", 1);

            await UserRepository.AddAsync(user, CancellationToken.None);
            var result = await UserRepository.GetAsync(user.Id, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Teste User");
            result.Email.Should().Be(email);
            result.Status.Should().Be('A');
        }

        [Fact]
        public async Task GetAsync_ById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var result = await UserRepository.GetAsync(999999, CancellationToken.None);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsync_ByEmail_ReturnsUser()
        {
            var shortId = Guid.NewGuid().ToString()[..6];
            var email = $"b_{shortId}@fiap.com";
            var user = new User("Busca Email", email, "Senha123!", 1);
            await UserRepository.AddAsync(user, CancellationToken.None);

            var result = await UserRepository.GetAsync(email, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Email.Should().Be(email);
        }

        [Fact]
        public async Task GetAsync_ByEmail_ShouldReturnNull_WhenEmailDoesNotExist()
        {
            var result = await UserRepository.GetAsync("naoexistente@fiap.com", CancellationToken.None);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsync_OnlyReturnsActiveUsers()
        {
            var shortActive = Guid.NewGuid().ToString()[..6];
            var shortInactive = Guid.NewGuid().ToString()[..6];

            var activeEmail = $"a_{shortActive}@fiap.com";
            var inactiveEmail = $"i_{shortInactive}@fiap.com";

            var activeUser = new User("Ativo", activeEmail, "Senha123!", 1);
            var inactiveUser = new User("Inativo", inactiveEmail, "Senha123!", 1);

            inactiveUser.Deactivate();

            await UserRepository.AddAsync(activeUser, CancellationToken.None);
            await UserRepository.AddAsync(inactiveUser, CancellationToken.None);

            var result = await UserRepository.GetAsync(CancellationToken.None);

            result.Should().Contain(u => u.Email == activeEmail);
            result.Should().NotContain(u => u.Email == inactiveEmail);
        }
    }
}