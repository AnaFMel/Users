using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Infra.Data.Contexts;

namespace Users.Infra.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(MySqlContext context) : base(context)
        {
        }

        public new async Task<User?> GetAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        public async Task<User?> GetAsync(string email, CancellationToken cancellationToken = default) => await _dbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        public new async Task<IEnumerable<User>> GetAsync(CancellationToken cancellationToken = default) => await _dbSet.Include(u => u.Role).Where(u => u.Status == 'A').ToListAsync(cancellationToken);
    }
}
