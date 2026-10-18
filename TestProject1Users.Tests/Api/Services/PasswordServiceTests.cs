using FluentAssertions;
using Users.Domain.Services;

namespace Users.Tests.Api.Services
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _passwordService = new();

        [Fact]
        public void CreateHash_ShouldReturnHashedPassword_NotEqualToOriginal()
        {
            // Arrange
            var rawPassword = "Password123!";

            // Act
            var hash = _passwordService.CreateHash(rawPassword);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hash.Should().NotBe(rawPassword);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var rawPassword = "Password123!";
            var hash = _passwordService.CreateHash(rawPassword);

            // Act
            var isValid = _passwordService.VerifyPassword(rawPassword, hash);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            var rawPassword = "Password123!";
            var wrongPassword = "WrongPassword123!";
            var hash = _passwordService.CreateHash(rawPassword);

            // Act
            var isValid = _passwordService.VerifyPassword(wrongPassword, hash);

            // Assert
            isValid.Should().BeFalse();
        }
    }
}