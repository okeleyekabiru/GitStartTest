using GitStartFramework.Shared.Persistence.Repository.interfaces;

namespace Product.API.Domain.Entities
{
    public class Category : IEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}