using System.Linq.Expressions;

namespace GitStartFramework.Shared.Persistence.Repository.interfaces
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        Task<IQueryable<T>> GetAllAsync();

        Task<T> GetByIdAsync(object id);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task UpdateAsync(T entity);

        Task DeleteAsync(object id);
    }
}