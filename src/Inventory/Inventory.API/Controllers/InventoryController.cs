using Microsoft.AspNetCore.Mvc;
using Inventory.API.Services;
using Inventory.API.Model; // Assuming you have DTOs defined here
using GitStartFramework.Shared.Model;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _inventoryService.GetAllInventoryItemsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(Guid id)
        {
            var result = await _inventoryService.GetInventoryItemByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(InventoryItemDto itemDto)
        {
            var result = await _inventoryService.CreateInventoryItemAsync(itemDto);
            return CreatedAtAction(nameof(GetItemById), new { id = result.Data.Id }, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateItem(UpdateInventoryItemDto itemDto)
        {
            var result = await _inventoryService.UpdateInventoryItemAsync(itemDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var result = await _inventoryService.DeleteInventoryItemAsync(id);
            return Ok(result);
        }

        [HttpPost("transaction")]
        public async Task<IActionResult> RecordTransaction(StockTransactionDto transactionDto)
        {
            var result = await _inventoryService.RecordStockTransactionAsync(transactionDto);
            return CreatedAtAction(nameof(RecordTransaction), result);
        }

        [HttpGet("{inventoryItemId}/transactions")]
        public async Task<IActionResult> GetTransactions(Guid inventoryItemId)
        {
            var result = await _inventoryService.GetTransactionsForItemAsync(inventoryItemId);
            return Ok(result);
        }
    }
}