using GitStartFramework.Shared.Exceptions;
using GitStartFramework.Shared.Model;
using GitStartFramework.Shared.Persistence.Repository.interfaces;
using Product.API.Model;

namespace Product.API.Service
{
    public interface IProductService
    {
        Task<Response<PaginatedResult<Domain.Entities.Product>>> GetAllProductsAsync(int pageNumber, int pageSize);

        Task<Response<Domain.Entities.Product?>> GetProductByIdAsync(Guid id);

        Task<Response<Domain.Entities.Product>> CreateProductAsync(ProductDto product);

        Task<Response<bool>> UpdateProductAsync(UpdateProductDto product);

        Task<Response<bool>> DeleteProductAsync(Guid id);
    }

    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Domain.Entities.Product> _productRepository;

        public ProductService(IGenericRepository<Domain.Entities.Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Response<PaginatedResult<Domain.Entities.Product>>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            var totalItems = (await _productRepository.GetAllAsync()).Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = (await _productRepository.GetAllAsync())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Response<PaginatedResult<Domain.Entities.Product>>.Success(new PaginatedResult<Domain.Entities.Product>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = products
            });
        }

        public async Task<Response<Domain.Entities.Product?>> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
            {
                throw new NotFoundException("Product not found");
            }
            return Response<Domain.Entities.Product>.Success(product);
        }

        public async Task<Response<Domain.Entities.Product>> CreateProductAsync(ProductDto productDto)
        {
            var product = new Domain.Entities.Product
            {
                DateCreated = DateTime.Now,
                StockQuantity = productDto.StockQuantity,
                Description = productDto.Description,
                Name = productDto.Name,
                IsActive = true,
                Price = productDto.Price
            };
            await _productRepository.AddAsync(product);
            return Response<Domain.Entities.Product>.Success(product);
        }

        public async Task<Response<bool>> UpdateProductAsync(UpdateProductDto productToUpdate)
        {
            var product = await GetProductByIdAsync(productToUpdate.Id.Value);
            product.Data.Price = productToUpdate.Price ?? product.Data.Price;
            product.Data.Name = productToUpdate.Name ?? product.Data.Name;
            product.Data.Description = productToUpdate.Description ?? product.Data.Description;
            product.Data.StockQuantity = productToUpdate.StockQuantity ?? product.Data.StockQuantity;
            product.Data.DateUpdated = DateTime.Now;

            await _productRepository.UpdateAsync(product.Data);
            return Response<bool>.Success(true);
        }

        public async Task<Response<bool>> DeleteProductAsync(Guid id)
        {
            await _productRepository.DeleteAsync(id);
            return Response<bool>.Success(true);
        }
    }
}