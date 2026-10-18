using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Users.Domain.Services.Security;

namespace Users.Tests.Api.Services
{
    public class UserServiceTests
    {
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly PasswordService _passwordService = new();
        private readonly JwtService _jwtService = new();
        private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
        private readonly ILogger<UserService> _logger = Substitute.For<ILogger<UserService>>();
        private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();

        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userService = new UserService(
                _userRepository,
                _passwordService,
                _jwtService,
                _publishEndpoint,
                _logger,
                _configuration
            );
        }

        [Fact]
        public async Task Add_Should_HashPassword_SaveUser_AndPublishEvent()
        {
            var user = new User("Admin User", "admin@fiap.com", "PlainPassword123", 1);

            await _userService.Add(user, CancellationToken.None);

            user.Password.Should().NotBe("PlainPassword123");
            user.Password.Should().NotBeNullOrEmpty();
            await _userRepository.Received(1).AddAsync(user, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Auth_WithValidCredentials_ShouldReturnToken()
        {
            var email = "user@fiap.com";
            var password = "ValidPassword123";
            var hashedPassword = _passwordService.CreateHash(password);

            var user = new User("Test User", email, hashedPassword, 1);
            typeof(BaseEntity).GetProperty("Status")?.SetValue(user, 'A');

            var roleObj = new Role(1, "Admin"); 
            typeof(User).GetProperty("Role")?.SetValue(user, roleObj);

            _userRepository.GetAsync(email, Arg.Any<CancellationToken>()).Returns(user);

            var token = await _userService.Auth(email, password, CancellationToken.None);

            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Auth_WithInvalidEmail_ShouldThrowUnauthorizedAccessException()
        {
            var email = "notfound@fiap.com";
            _userRepository.GetAsync(email, Arg.Any<CancellationToken>()).Returns((User?)null);

            Func<Task> act = async () => await _userService.Auth(email, "password", CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User and/or password are invalid.");
        }

        [Fact]
        public async Task Auth_WithInactiveUser_ShouldThrowUnauthorizedAccessException()
        {
            var email = "inactive@fiap.com";
            var hashedPassword = _passwordService.CreateHash("ValidPassword123");
            var user = new User("Inactive", email, hashedPassword, 1);

            typeof(BaseEntity).GetProperty("Status")?.SetValue(user, 'I');

            _userRepository.GetAsync(email, Arg.Any<CancellationToken>()).Returns(user);

            Func<Task> act = async () => await _userService.Auth(email, "ValidPassword123", CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Auth_WithInvalidPassword_ShouldThrowUnauthorizedAccessException()
        {
            var email = "user@fiap.com";
            var hashedPassword = _passwordService.CreateHash("ValidPassword123");
            var user = new User("Test User", email, hashedPassword, 1);

            typeof(BaseEntity).GetProperty("Status")?.SetValue(user, 'A');

            _userRepository.GetAsync(email, Arg.Any<CancellationToken>()).Returns(user);

            Func<Task> act = async () => await _userService.Auth(email, "WrongPassword", CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User and/or password are invalid.");
        }
    }
}