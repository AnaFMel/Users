using Users.Domain.Entities;
using Users.Domain.Repositories;

namespace Users.Domain.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordService _passwordService;

        public UserService(IUserRepository userRepository, PasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
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
