using Users.Domain.Entities;

namespace Users.Domain.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeactivateAsync(int id, CancellationToken cancellationToken = default);
    }
}
