using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Users.Domain.Services.Security;

namespace Users.Tests.Api.Services.Security
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService = new();

        [Fact]
        public void GenerateToken_ShouldReturnValidJwtToken_WithCorrectClaims()
        {
            var email = "admin@fiap.com";
            var name = "Admin User";
            var role = "Admin";

            var tokenString = _jwtService.GenerateToken(email, name, role);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);

            jwtToken.Should().NotBeNull();

            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Value == name)?.Value;

            nameClaim.Should().Be(name);
        }
    }
}