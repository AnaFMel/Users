using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Users.Infra.CrossCutting.Security
{
    public class JwtService
    {
        private readonly string jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? "M72gsn/ZtfvDSgyBgWNBOYf8HAHBO7FGIwiKPtwipmdMY6ZbdgrJ66yBLnXKk9O8P6fjj7G+BoE5+Ou/xfEgNQ==";
        private readonly string[] jwtIssuer = [Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "FiapCloudGames"];
        private readonly int jwtSeconds = Convert.ToInt32(Environment.GetEnvironmentVariable("JWT_SECONDS") ?? "3600");

        public string GenerateToken(string email, string name, string role)
        {
            var claimsIdentity = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, email),
                    new Claim(ClaimTypes.Actor, name),
                    new Claim(ClaimTypes.Role, role)
                ]
            );

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = signingCredentials,
                Subject = claimsIdentity,
                Issuer = jwtIssuer[0],
                Audience = name,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now + TimeSpan.FromSeconds(jwtSeconds)
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }
    }
}
