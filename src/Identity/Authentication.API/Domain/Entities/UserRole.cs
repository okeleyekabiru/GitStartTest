using GitStartFramework.Shared.Persistence.Repository.interfaces;

namespace Authentication.API.Domain.Entities
{
    public class UserRole : IEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public virtual User? User { get; set; }
        public virtual Role? Role { get; set; }
    }
}