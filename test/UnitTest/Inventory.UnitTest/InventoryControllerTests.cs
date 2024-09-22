using Microsoft.AspNetCore.Mvc;
using Moq;
using Inventory.API.Controllers;
using Inventory.API.Services;
using Inventory.API.Model;
using GitStartFramework.Shared.Model;
using Inventory.API.Domain.Entities;

namespace Inventory.IntegrationTest
{
    public class InventoryControllerTests
    {
        private readonly Mock<IInventoryService> _mockInventoryService;
        private readonly InventoryController _controller;

        public InventoryControllerTests()
        {
            _mockInventoryService = new Mock<IInventoryService>();
            _controller = new InventoryController(_mockInventoryService.Object);
        }

        [Fact]
        public async Task GetAllItems_ReturnsOkResult()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var paginatedResult = new PaginatedResult<InventoryItem>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = 20,
                TotalPages = 2,
                Items = new List<InventoryItem>()
            };

            _mockInventoryService.Setup(service => service.GetAllInventoryItemsAsync(pageNumber, pageSize))
                .ReturnsAsync(Response<PaginatedResult<InventoryItem>>.Success(paginatedResult));

            // Act
            var result = await _controller.GetAllItems(pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<PaginatedResult<InventoryItem>>>(okResult.Value);
            Assert.Equal(20, response.Data.TotalItems);
        }

        [Fact]
        public async Task CreateItem_ReturnsCreatedAtAction()
        {
            // Arrange
            var itemDto = new InventoryItemDto { ProductId = Guid.NewGuid(), QuantityInStock = 15 };
            var createdItem = new InventoryItem { Id = Guid.NewGuid(), ProductId = itemDto.ProductId, QuantityInStock = itemDto.QuantityInStock };

            _mockInventoryService.Setup(service => service.CreateInventoryItemAsync(itemDto))
                .ReturnsAsync(Response<InventoryItem>.Success(createdItem));

            // Act
            var result = await _controller.CreateItem(itemDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetItemById", createdResult.ActionName);
        }
    }
}