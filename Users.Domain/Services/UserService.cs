using Fcg.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Domain.Services.Security;

namespace Users.Domain.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordService _passwordService;
        private readonly JwtService _jwtService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, PasswordService passwordService, JwtService jwtService, IPublishEndpoint publishEndpoint, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }


        public async Task Add(User user, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Iniciando cadastro do novo usuário '{user.Name}'. Email: {user.Email}.");

            user.SetPassword(_passwordService.CreateHash(user.Password));

            await _userRepository.AddAsync(user, cancellationToken);

            _logger.LogInformation($"Usuário '{user.Name}' ({user.Email}) cadastrado com sucesso! Id: {user.Id}.");

            await _publishEndpoint.Publish(new UserCreatedEvent(user.Id, user.Email, user.Name, user.RoleId), cancellationToken);

            _logger.LogInformation("Evento 'UserCreatedEvent' publicado com sucesso!" +
                                   $"\nUserId: {user.Id}." +
                                   $"\nUserName: {user.Name}." +
                                   $"\nUserEmail: {user.Email}." +
                                   $"\nUserRole: {user.RoleId}.");
        }

        public async Task<string> Auth(string email, string password, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Iniciando autenticação do usuário. Email: {email}.");

                var user = await _userRepository.GetAsync(email, cancellationToken) ?? throw new UnauthorizedAccessException("User and/or password are invalid.");

                if (user.Status != 'A') throw new UnauthorizedAccessException("User is inactive.");

                var validPassword = _passwordService.VerifyPassword(password, user.Password);

                if (!validPassword) throw new UnauthorizedAccessException("User and/or password are invalid.");

                var token = _jwtService.GenerateToken(user.Email, user.Name, user.Role!.Name);

                _logger.LogInformation($"O usuário '{user.Name}' ({email}) foi autenticado com sucesso! Id: {user.Id}.");

                return token;
            }
            catch (Exception)
            {
                _logger.LogInformation($"As credenciais inseridas são inválidas! Email: {email}.");
                throw;
            }
        }
    }
}

