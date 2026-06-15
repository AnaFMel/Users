using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Users.Infra.CrossCutting.Security;

namespace Users.API.Configurations
{
    public static class JwtSecurityExtensions
    {
        public static IServiceCollection AddJwtSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>() ?? throw new Exception("JwtOptions não encontrado."); ;

            services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                var bytes = Encoding.UTF8.GetBytes(jwtOptions.Key);
                var symmetricSecurityKey = new SymmetricSecurityKey(bytes);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = symmetricSecurityKey,
                    ValidIssuers = jwtOptions.Issuers,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context => { return Task.CompletedTask; },
                    OnAuthenticationFailed = context => { return Task.CompletedTask; },
                    OnChallenge = context => { return Task.CompletedTask; }
                };
            });

            return services;
        }
    }
}
