using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Infra.Data.Contexts;

namespace Users.Infra.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        protected readonly MySqlContext _context;
        protected readonly DbSet<User> _dbSet;

        public UserRepository(MySqlContext context)
        {
            _context = context;
            _dbSet = _context.Set<User>();
        }

        public async Task<User?> GetAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        public async Task<User?> GetAsync(string email, CancellationToken cancellationToken = default) => await _dbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        public async Task<IEnumerable<User>> GetAsync(CancellationToken cancellationToken = default) => await _dbSet.Include(u => u.Role).Where(u => u.Status == 'A').ToListAsync(cancellationToken);
        public async Task AddAsync(User user, CancellationToken cancellationToken) => await _context.Users.AddAsync(user, cancellationToken);
        public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
    }
}
