using Inventory.API.Domain.Entities;
using GitStartFramework.Shared.Model;
using GitStartFramework.Shared.Exceptions;
using GitStartFramework.Shared.Persistence.Repository.interfaces;
using Inventory.API.Model;

namespace Inventory.API.Services
{
    public interface IInventoryService
    {
        Task<Response<PaginatedResult<InventoryItem>>> GetAllInventoryItemsAsync(int pageNumber, int pageSize);

        Task<Response<InventoryItem?>> GetInventoryItemByIdAsync(Guid id);

        Task<Response<InventoryItem>> CreateInventoryItemAsync(InventoryItemDto itemDto);

        Task<Response<bool>> UpdateInventoryItemAsync(UpdateInventoryItemDto itemDto);

        Task<Response<bool>> DeleteInventoryItemAsync(Guid id);

        Task<Response<StockTransaction>> RecordStockTransactionAsync(StockTransactionDto transactionDto);

        Task<Response<IEnumerable<StockTransaction>>> GetTransactionsForItemAsync(Guid inventoryItemId);
    }

    public class InventoryService : IInventoryService
    {
        private readonly IGenericRepository<InventoryItem> _inventoryRepository;
        private readonly IGenericRepository<StockTransaction> _transactionRepository;

        public InventoryService(
            IGenericRepository<InventoryItem> inventoryRepository,
            IGenericRepository<StockTransaction> transactionRepository)
        {
            _inventoryRepository = inventoryRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<Response<PaginatedResult<InventoryItem>>> GetAllInventoryItemsAsync(int pageNumber, int pageSize)
        {
            var totalItems = (await _inventoryRepository.GetAllAsync()).Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = (await _inventoryRepository.GetAllAsync())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Response<PaginatedResult<InventoryItem>>.Success(new PaginatedResult<InventoryItem>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            });
        }

        public async Task<Response<InventoryItem?>> GetInventoryItemByIdAsync(Guid id)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item is null)
            {
                throw new NotFoundException("Inventory item not found");
            }
            return Response<InventoryItem>.Success(item);
        }

        public async Task<Response<InventoryItem>> CreateInventoryItemAsync(InventoryItemDto itemDto)
        {
            var item = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = itemDto.ProductId,
                QuantityInStock = itemDto.QuantityInStock,
                LastRestocked = DateTime.UtcNow,
                DateCreated = DateTime.UtcNow
            };
            await _inventoryRepository.AddAsync(item);
            return Response<InventoryItem>.Success(item);
        }

        public async Task<Response<bool>> UpdateInventoryItemAsync(UpdateInventoryItemDto itemDto)
        {
            var item = await GetInventoryItemByIdAsync(itemDto.Id.Value);
            item.Data.QuantityInStock = itemDto.QuantityInStock ?? item.Data.QuantityInStock;
            item.Data.LastRestocked = DateTime.UtcNow;

            await _inventoryRepository.UpdateAsync(item.Data);
            return Response<bool>.Success(true);
        }

        public async Task<Response<bool>> DeleteInventoryItemAsync(Guid id)
        {
            await _inventoryRepository.DeleteAsync(id);
            return Response<bool>.Success(true);
        }

        public async Task<Response<StockTransaction>> RecordStockTransactionAsync(StockTransactionDto transactionDto)
        {
            var transaction = new StockTransaction
            {
                Id = Guid.NewGuid(),
                InventoryItemId = transactionDto.InventoryItemId,
                QuantityChanged = transactionDto.QuantityChanged,
                DateOfTransaction = DateTime.UtcNow,
                TransactionType = transactionDto.TransactionType,
                Reason = transactionDto.Reason
            };

            await _transactionRepository.AddAsync(transaction);
            return Response<StockTransaction>.Success(transaction);
        }

        public async Task<Response<IEnumerable<StockTransaction>>> GetTransactionsForItemAsync(Guid inventoryItemId)
        {
            var transactions = await _transactionRepository.FindAsync(t => t.InventoryItemId == inventoryItemId);
            return Response<IEnumerable<StockTransaction>>.Success(transactions);
        }
    }
}