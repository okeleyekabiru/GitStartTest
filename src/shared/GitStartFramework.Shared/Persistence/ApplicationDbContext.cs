using Microsoft.EntityFrameworkCore;

namespace GitStartFramework.Shared.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }
    }
}