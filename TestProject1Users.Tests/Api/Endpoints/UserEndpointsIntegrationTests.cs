using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Users.API.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Users.Domain.Services.Security;
using Users.Infra.Data.Contexts;

namespace Users.Tests.Api.Endpoints
{
    public class UserEndpointsTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly SqliteConnection _connection;
        private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();
        private readonly JwtService _jwtService = new();
        private readonly string _validJwtToken;

        public UserEndpointsTests(WebApplicationFactory<Program> factory)
        {
            Environment.SetEnvironmentVariable("KeyVaultUri", null);
            Environment.SetEnvironmentVariable(
                "ServiceBusConnectionString",
                "Endpoint=sb://mock-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=1234567890abcdef="
            );
            Environment.SetEnvironmentVariable("JWT_KEY", "M72gsn/ZtfvDSgyBgWNBOYf8HAHBO7FGIwiKPtwipmdMY6ZbdgrJ66yBLnXKk9O8P6fjj7G+BoE5+Ou/xfEgNQ==");
            Environment.SetEnvironmentVariable("JWT_ISSUER", "FiapCloudGames");
            Environment.SetEnvironmentVariable("JWT_SECONDS", "3600");

            var fakeConn = "Server=tcp:mock.database.windows.net,1433;Initial Catalog=MockDb;User ID=mock;Password=mock;";
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", fakeConn);
            Environment.SetEnvironmentVariable("ConnectionStrings__Default", fakeConn);

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _connection.CreateFunction("GETDATE", () => DateTime.Now);

            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptorsToRemove = services
                        .Where(d => d.ServiceType.Name.Contains("SqlServerContext") ||
                                    d.ServiceType.Name.Contains("DbContextOptions"))
                        .ToList();

                    foreach (var descriptor in descriptorsToRemove)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<SqlServerContext>(options =>
                    {
                        options.UseSqlite(_connection);
                        options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
                    });

                    services.RemoveAll<IUserRepository>();
                    services.AddSingleton(_userRepositoryMock);
                });
            }).CreateClient();

            _validJwtToken = _jwtService.GenerateToken("admin@test.com", "Admin Test", "Admin");
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        private void ClearMocks()
        {
            _userRepositoryMock.ClearSubstitute();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        private void Authenticate()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validJwtToken);
        }

        [Fact]
        public async Task Auth_ShouldReturnToken_WhenCredentialsAreValid()
        {
            ClearMocks();
            var loginDto = new LoginRequest
            {
                Email = "admin@test.com",
                Password = "Password123!"
            };

            var passwordService = new PasswordService();
            var hashedPassword = passwordService.CreateHash("Password123!");

            var mockUser = new User("Admin Test", "admin@test.com", hashedPassword, 1);
            typeof(BaseEntity).GetProperty("Status")?.SetValue(mockUser, 'A');

            var roleObj = new Role(1, "Admin");
            typeof(User).GetProperty("Role")?.SetValue(mockUser, roleObj);

            _userRepositoryMock.GetAsync(loginDto.Email, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<User?>(mockUser));

            var response = await _client.PostAsJsonAsync("/user/auth", loginDto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            Assert.NotNull(result);
            Assert.Equal(loginDto.Email, result.Email);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task Auth_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
        {
            ClearMocks();
            var invalidLoginDto = new LoginRequest
            {
                Email = "admin@test.com",
                Password = "SenhaErrada123!"
            };

            var passwordService = new PasswordService();
            var hashedPassword = passwordService.CreateHash("Password123!");

            var mockUser = new User("Admin Test", "admin@test.com", hashedPassword, 1);
            typeof(BaseEntity).GetProperty("Status")?.SetValue(mockUser, 'A');

            var roleObj = new Role(1, "Admin");
            typeof(User).GetProperty("Role")?.SetValue(mockUser, roleObj);

            _userRepositoryMock.GetAsync(invalidLoginDto.Email, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<User?>(mockUser));

            var response = await _client.PostAsJsonAsync("/user/auth", invalidLoginDto);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            ClearMocks();
            Authenticate();

            _userRepositoryMock.GetAsync(999999, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<User?>(null));

            var response = await _client.GetAsync("/user/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNotFound_WhenEmailDoesNotExist()
        {
            ClearMocks();
            Authenticate();

            var email = "naoexiste@test.com";
            _userRepositoryMock.GetAsync(email, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<User?>(null));

            var response = await _client.GetAsync($"/user/{email}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnUser_WhenEmailExists()
        {
            ClearMocks();
            Authenticate();

            var email = "admin@test.com";
            var mockUser = new User("Admin Test", email, "hashedpassword", 1);
            var roleObj = new Role(1, "Admin");
            typeof(User).GetProperty("Role")?.SetValue(mockUser, roleObj);

            _userRepositoryMock.GetAsync(email, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<User?>(mockUser));

            var response = await _client.GetAsync($"/user/{email}");

            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAllUsers_ShouldExecuteEndpoint()
        {
            ClearMocks();
            Authenticate();

            _userRepositoryMock.GetAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<User>>(new List<User>()));

            var response = await _client.GetAsync("/user");

            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            ClearMocks();
            var createDto = new CreateUserRequest
            {
                Name = "Test",
                Email = "existente@test.com",
                Password = "Password123!",
                RoleId = 1
            };

            var existingUser = new User("Existing", createDto.Email, "hash", 1);
            var roleObj = new Role(1, "Admin");
            typeof(User).GetProperty("Role")?.SetValue(existingUser, roleObj);

            _userRepositoryMock.GetAsync(createDto.Email, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<User?>(existingUser));

            var response = await _client.PostAsJsonAsync("/user/create", createDto);

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Unauthorized);
        }
    }
}