namespace Inventory.API.Model
{
    public class InventoryItemDto
    {
        public Guid ProductId { get; set; }
        public int QuantityInStock { get; set; }
    }

    public class UpdateInventoryItemDto
    {
        public Guid? Id { get; set; }
        public int? QuantityInStock { get; set; }
    }

    public class StockTransactionDto
    {
        public Guid InventoryItemId { get; set; }
        public int QuantityChanged { get; set; }
        public string TransactionType { get; set; }
        public string Reason { get; set; }
    }
}