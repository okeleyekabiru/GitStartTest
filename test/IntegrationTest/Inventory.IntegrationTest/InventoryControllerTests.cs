using Microsoft.AspNetCore.Mvc;
using Moq;
using Inventory.API.Controllers;
using Inventory.API.Services;
using Inventory.API.Model;
using GitStartFramework.Shared.Model;
using GitStartFramework.Shared.Exceptions;
using Inventory.API.Domain.Entities;
using System;

namespace Inventory.Tests
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

            var result = await _controller.GetAllItems(pageNumber, pageSize);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<PaginatedResult<InventoryItem>>>(okResult.Value);
            Assert.Equal(20, response.Data.TotalItems);
        }

        [Fact]
        public async Task GetItemById_ReturnsOkResult()
        {
            var itemId = Guid.NewGuid();
            var item = new InventoryItem { Id = itemId, ProductId = Guid.NewGuid(), QuantityInStock = 10 };

            _mockInventoryService.Setup(service => service.GetInventoryItemByIdAsync(itemId))
                .ReturnsAsync(Response<InventoryItem>.Success(item));

            var result = await _controller.GetItemById(itemId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<InventoryItem>>(okResult.Value);
            Assert.Equal(itemId, response.Data.Id);
        }

        [Fact]
        public async Task GetItemById_ItemNotFound_ReturnsNotFound()
        {
            var itemId = Guid.NewGuid();
            _mockInventoryService.Setup(service => service.GetInventoryItemByIdAsync(itemId))
                .ThrowsAsync(new NotFoundException("Item not found"));

            await Assert.ThrowsAsync<NotFoundException>(
                 async () => await _controller.GetItemById(itemId));
        }

        [Fact]
        public async Task CreateItem_ReturnsCreatedAtAction()
        {
            var itemDto = new InventoryItemDto { ProductId = Guid.NewGuid(), QuantityInStock = 15 };
            var createdItem = new InventoryItem { Id = Guid.NewGuid(), ProductId = itemDto.ProductId, QuantityInStock = itemDto.QuantityInStock };

            _mockInventoryService.Setup(service => service.CreateInventoryItemAsync(itemDto))
                .ReturnsAsync(Response<InventoryItem>.Success(createdItem));

            var result = await _controller.CreateItem(itemDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetItemById", createdResult.ActionName);
        }

        [Fact]
        public async Task DeleteItem_ReturnsOkResult()
        {
            var itemId = Guid.NewGuid();
            _mockInventoryService.Setup(service => service.DeleteInventoryItemAsync(itemId))
                .ReturnsAsync(Response<bool>.Success(true));

            var result = await _controller.DeleteItem(itemId);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}