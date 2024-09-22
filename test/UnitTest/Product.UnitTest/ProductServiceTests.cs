using Moq;
using Product.API.Model;
using GitStartFramework.Shared.Model;
using GitStartFramework.Shared.Persistence.Repository.interfaces;
using Product.API.Service;
using GitStartFramework.Shared.Exceptions;

namespace Product.IntegrationTest
{
    public class ProductServiceTests
    {
        private readonly Mock<IGenericRepository<API.Domain.Entities.Product>> _mockProductRepo;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepo = new Mock<IGenericRepository<API.Domain.Entities.Product>>();
            _productService = new ProductService(_mockProductRepo.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsPaginatedResult()
        {
            
            var products = Task.FromResult(new List<API.Domain.Entities.Product>
            {
                new API.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Product 1", Price = 10.00m, StockQuantity = 5 },
                new API.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Product 2", Price = 20.00m, StockQuantity = 15 }
            }.AsQueryable());

            _mockProductRepo.Setup(repo => repo.GetAllAsync()).Returns(products);

            
            var result = await _productService.GetAllProductsAsync(1, 10);

            
            Assert.NotNull(result);
            Assert.IsType<Response<PaginatedResult<Product.API.Domain.Entities.Product>>>(result);
            Assert.Equal(2, result.Data.TotalItems);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsProduct()
        {
            
            var productId = Guid.NewGuid();
            var product = new Product.API.Domain.Entities.Product { Id = productId, Name = "Product 1", Price = 10.00m, StockQuantity = 5 };

            _mockProductRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            
            var result = await _productService.GetProductByIdAsync(productId);

            
            Assert.NotNull(result);
            Assert.IsType<Response<Product.API.Domain.Entities.Product>>(result);
            Assert.Equal(productId, result.Data.Id);
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductNotFound_ThrowsNotFoundException()
        {
            
            var productId = Guid.NewGuid();
            _mockProductRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((Product.API.Domain.Entities.Product)null);

            await Assert.ThrowsAsync<NotFoundException>(async () => await _productService.GetProductByIdAsync(productId));
        }

        [Fact]
        public async Task CreateProductAsync_CreatesProduct()
        {
            
            var productDto = new ProductDto { Name = "New Product", Price = 30.00m, StockQuantity = 10 };
            var newProduct = new Product.API.Domain.Entities.Product { Id = Guid.NewGuid(), Name = productDto.Name, Price = productDto.Price, StockQuantity = productDto.StockQuantity };

            _mockProductRepo.Setup(repo => repo.AddAsync(It.IsAny<API.Domain.Entities.Product>())).Returns(Task.CompletedTask);

            
            var result = await _productService.CreateProductAsync(productDto);

            
            Assert.NotNull(result);
            Assert.IsType<Response<API.Domain.Entities.Product>>(result);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdatesProduct()
        {
            
            var productId = Guid.NewGuid();
            var existingProduct = new API.Domain.Entities.Product { Id = productId, Name = "Old Product", Price = 20.00m, StockQuantity = 5 };
            var updateProductDto = new UpdateProductDto { Id = productId, Name = "Updated Product", Price = 25.00m };

            _mockProductRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(existingProduct);
            _mockProductRepo.Setup(repo => repo.UpdateAsync(It.IsAny<API.Domain.Entities.Product>())).Returns(Task.CompletedTask);

            
            var result = await _productService.UpdateProductAsync(updateProductDto);

            
            Assert.NotNull(result);
            Assert.IsType<Response<bool>>(result);
        }

        [Fact]
        public async Task UpdateProductAsync_ProductNotFound_ThrowsNotFoundException()
        {
            
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { Id = productId, Name = "Updated Product", Price = 25.00m };

            _mockProductRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((Product.API.Domain.Entities.Product)null);

            await Assert.ThrowsAsync<NotFoundException>(async () => await _productService.UpdateProductAsync(updateProductDto));
        }

        [Fact]
        public async Task DeleteProductAsync_DeletesProduct()
        {
            
            var productId = Guid.NewGuid();
            _mockProductRepo.Setup(repo => repo.DeleteAsync(productId)).Returns(Task.CompletedTask);

            
            var result = await _productService.DeleteProductAsync(productId);

            
            Assert.NotNull(result);
            Assert.IsType<Response<bool>>(result);
        }

        [Fact]
        public async Task DeleteProductAsync_ProductNotFound_ThrowsNotFoundException()
        {
            
            var productId = Guid.NewGuid();
            _mockProductRepo.Setup(repo => repo.DeleteAsync(productId)).ThrowsAsync(new NotFoundException("Product not found"));

            await Assert.ThrowsAsync<NotFoundException>(async () => await _productService.DeleteProductAsync(productId));
        }
    }
}