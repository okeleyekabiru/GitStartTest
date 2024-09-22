using Inventory.API.Domain.Entities;
using Inventory.API.Model;
using Inventory.API.Services;
using GitStartFramework.Shared.Model;
using GitStartFramework.Shared.Persistence.Repository.interfaces;
using Moq;
using GitStartFramework.Shared.Exceptions;

namespace Inventory.IntegrationTest
{
    public class InventoryServiceTests
    {
        private readonly Mock<IGenericRepository<InventoryItem>> _mockInventoryRepo;
        private readonly Mock<IGenericRepository<StockTransaction>> _mockTransactionRepo;
        private readonly InventoryService _inventoryService;

        public InventoryServiceTests()
        {
            _mockInventoryRepo = new Mock<IGenericRepository<InventoryItem>>();
            _mockTransactionRepo = new Mock<IGenericRepository<StockTransaction>>();
            _inventoryService = new InventoryService(_mockInventoryRepo.Object, _mockTransactionRepo.Object);
        }

        [Fact]
        public async Task GetAllInventoryItemsAsync_ReturnsPaginatedResult()
        {
            var items = Task.FromResult(new List<InventoryItem>
            {
                new InventoryItem { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), QuantityInStock = 10 },
                new InventoryItem { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), QuantityInStock = 20 },
            }.AsQueryable());

            _mockInventoryRepo.Setup(repo => repo.GetAllAsync()).Returns(items);

            var result = await _inventoryService.GetAllInventoryItemsAsync(1, 10);

            Assert.NotNull(result);
            Assert.IsType<Response<PaginatedResult<InventoryItem>>>(result);
            Assert.Equal(2, result.Data.TotalItems);
        }

        [Fact]
        public async Task GetInventoryItemByIdAsync_ReturnsItem()
        {
            var itemId = Guid.NewGuid();
            var item = new InventoryItem { Id = itemId, ProductId = Guid.NewGuid(), QuantityInStock = 10 };

            _mockInventoryRepo.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(item);

            var result = await _inventoryService.GetInventoryItemByIdAsync(itemId);

            Assert.NotNull(result);
            Assert.IsType<Response<InventoryItem>>(result);
            Assert.Equal(itemId, result.Data.Id);
        }

        [Fact]
        public async Task GetInventoryItemByIdAsync_ItemNotFound_ThrowsNotFoundException()
        {
            var itemId = Guid.NewGuid();
            _mockInventoryRepo.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync((InventoryItem)null);

            await Assert.ThrowsAsync<NotFoundException>(async () => await _inventoryService.GetInventoryItemByIdAsync(itemId));
        }

        [Fact]
        public async Task CreateInventoryItemAsync_CreatesItem()
        {
            var itemDto = new InventoryItemDto { ProductId = Guid.NewGuid(), QuantityInStock = 15 };
            var newItem = new InventoryItem { Id = Guid.NewGuid(), ProductId = itemDto.ProductId, QuantityInStock = itemDto.QuantityInStock };

            _mockInventoryRepo.Setup(repo => repo.AddAsync(It.IsAny<InventoryItem>())).Returns(Task.CompletedTask);

            var result = await _inventoryService.CreateInventoryItemAsync(itemDto);

            Assert.NotNull(result);
            Assert.IsType<Response<InventoryItem>>(result);
        }

        [Fact]
        public async Task DeleteInventoryItemAsync_DeletesItem()
        {
            var itemId = Guid.NewGuid();
            _mockInventoryRepo.Setup(repo => repo.DeleteAsync(itemId)).Returns(Task.CompletedTask);

            var result = await _inventoryService.DeleteInventoryItemAsync(itemId);

            Assert.NotNull(result);
            Assert.IsType<Response<bool>>(result);
        }

        [Fact]
        public async Task DeleteInventoryItemAsync_ItemNotFound_ThrowsNotFoundException()
        {
            var itemId = Guid.NewGuid();
            _mockInventoryRepo.Setup(repo => repo.DeleteAsync(itemId)).ThrowsAsync(new NotFoundException("Item not found"));

            await Assert.ThrowsAsync<NotFoundException>(async () => await _inventoryService.DeleteInventoryItemAsync(itemId));
        }
    }
}