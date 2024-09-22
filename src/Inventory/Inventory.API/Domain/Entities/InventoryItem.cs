namespace Inventory.API.Domain.Entities
{
    public class InventoryItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int QuantityInStock { get; set; }
        public DateTime LastRestocked { get; set; }
        public DateTime DateCreated { get; set; }
    }
}