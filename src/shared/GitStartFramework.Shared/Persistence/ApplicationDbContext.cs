using GitStartFramework.Shared.Persistence.Repository.interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace GitStartFramework.Shared.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityAssembly = Assembly.Load(configuration["Application_Name"] ?? "Inventory.API");

            modelBuilder.ApplyConfigurationsFromAssembly(entityAssembly);
        }
    }
}