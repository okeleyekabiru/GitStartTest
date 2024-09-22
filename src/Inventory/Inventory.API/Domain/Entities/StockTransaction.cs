using GitStartFramework.Shared.Persistence.Repository.interfaces;

namespace Inventory.API.Domain.Entities
{
    public class StockTransaction : IEntity
    {
        public Guid Id { get; set; }
        public Guid InventoryItemId { get; set; }
        public InventoryItem? InventoryItem { get; set; }
        public int QuantityChanged { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public string? TransactionType { get; set; }
        public string? Reason { get; set; }
    }
}