using GitStartFramework.Shared.Persistence.Repository.interfaces;

namespace Authentication.API.Domain.Entities
{
    public class Role : IEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}