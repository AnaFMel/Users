using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Users.Infra.CrossCutting.Security
{
    public class JwtService
    {
        private readonly IOptions<JwtOptions> _jwtOptions;

        public JwtService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions;
        }

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

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = signingCredentials,
                Subject = claimsIdentity,
                Issuer = _jwtOptions.Value.Issuers[0],
                Audience = name,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now + TimeSpan.FromSeconds(_jwtOptions.Value.Seconds)
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }
    }
}
