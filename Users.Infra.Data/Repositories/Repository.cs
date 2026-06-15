using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Infra.Data.Contexts;

namespace Users.Infra.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly MySqlContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(MySqlContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.FindAsync([id], cancellationToken);

        public async Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken = default) => await _dbSet.Where(t => t.Status == 'A').ToListAsync(cancellationToken);

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await GetAsync(id, cancellationToken);

            if (entity != null)
            {
                entity.Deactivate();
                await UpdateAsync(entity, cancellationToken);
            }
            else
            {
                throw new Exception("The entity could not be found.");
            }
        }
    }
}
