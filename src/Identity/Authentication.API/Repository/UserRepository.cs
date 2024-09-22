using Authentication.API.Domain.Entities;
using GitStartFramework.Shared.Persistence;
using GitStartFramework.Shared.Persistence.Repository;
using GitStartFramework.Shared.Persistence.Repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace Authentication.API.Repository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        ValueTask<User?> GetUserByEmailAsync(string email);
    }

    public class UserRepository(ApplicationDbContext context) : GenericRepository<User>(context), IUserRepository
    {
        public async ValueTask<User?> GetUserByEmailAsync(string email)
        {
            return await DbSet.SingleOrDefaultAsync(e => e.Email == email);
        }
    }
}