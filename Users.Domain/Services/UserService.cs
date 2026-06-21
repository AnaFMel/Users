using MassTransit;
using Users.Domain.Entities;
using Users.Domain.Events;
using Users.Domain.Repositories;

namespace Users.Domain.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordService _passwordService;
        private readonly IPublishEndpoint _publishEndpoint;

        public UserService(IUserRepository userRepository, PasswordService passwordService, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _publishEndpoint = publishEndpoint;
        }


        public async Task Add(User user, CancellationToken cancellationToken)
        {
            user.SetPassword(_passwordService.CreateHash(user.Password));

            await _userRepository.AddAsync(user, cancellationToken);

            await _publishEndpoint.Publish(new UserCreatedEvent(user.Id, user.Email, user.Name, user.RoleId), cancellationToken);
        }

        public async Task<User> Auth(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(email, cancellationToken) ?? throw new UnauthorizedAccessException("User and/or password are invalid.");

            if (user.Status != 'A') throw new UnauthorizedAccessException("User is inactive.");

            var validPassword = _passwordService.VerifyPassword(password, user.Password);

            if (!validPassword) throw new UnauthorizedAccessException("User and/or password are invalid.");

            return user;
        }
    }
}

