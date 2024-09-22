using GitStartFramework.Shared.Persistence.Repository.interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GitStartFramework.Shared.Persistence.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly ApplicationDbContext _context;
        public readonly DbSet<T> DbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            DbSet = context.Set<T>();
        }

        public async Task<IQueryable<T>> GetAllAsync()
        {
            return DbSet.AsQueryable();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            DbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(object id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity != null)
            {
                DbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }
    }
}