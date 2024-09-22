using GitStartFramework.Shared.Persistence.Repository.interfaces;

namespace Authentication.API.Domain.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLogin { get; set; }
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}