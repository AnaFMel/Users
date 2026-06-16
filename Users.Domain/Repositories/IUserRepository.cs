using Users.Domain.Entities;

namespace Users.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<User?> GetAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAsync(CancellationToken cancellationToken = default);
        Task AddAsync(User entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
