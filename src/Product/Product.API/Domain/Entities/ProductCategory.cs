using GitStartFramework.Shared.Persistence.Repository.interfaces;

namespace Product.API.Domain.Entities
{
    public class ProductCategory : IEntity
    {
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}