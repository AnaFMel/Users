using Users.Domain.Entities;

namespace Users.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetAsync(string email, CancellationToken cancellationToken = default);
    }
}
