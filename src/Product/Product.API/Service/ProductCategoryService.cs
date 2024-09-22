using GitStartFramework.Shared.Exceptions;
using GitStartFramework.Shared.Model;
using GitStartFramework.Shared.Persistence.Repository.interfaces;
using Product.API.Model;

namespace Product.API.Service
{
    public interface IProductCategoryService
    {
        Task<Response<PaginatedResult<Domain.Entities.ProductCategory>>> GetAllProductCategoriesAsync(int pageNumber, int pageSize);

        Task<Response<Domain.Entities.ProductCategory?>> GetProductCategoryByIdAsync(Guid productId, Guid categoryId);

        Task<Response<Domain.Entities.ProductCategory>> CreateProductCategoryAsync(ProductCategoryDto productCategoryDto);

        Task<Response<bool>> DeleteProductCategoryAsync(Guid productId, Guid categoryId);
    }

    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IGenericRepository<Domain.Entities.ProductCategory> _productCategoryRepository;

        public ProductCategoryService(IGenericRepository<Domain.Entities.ProductCategory> productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
        }

        public async Task<Response<PaginatedResult<Domain.Entities.ProductCategory>>> GetAllProductCategoriesAsync(int pageNumber, int pageSize)
        {
            var totalItems = (await _productCategoryRepository.GetAllAsync()).Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var productCategories = (await _productCategoryRepository.GetAllAsync())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Response<PaginatedResult<Domain.Entities.ProductCategory>>.Success(new PaginatedResult<Domain.Entities.ProductCategory>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = productCategories
            });
        }

        public async Task<Response<Domain.Entities.ProductCategory?>> GetProductCategoryByIdAsync(Guid productId, Guid categoryId)
        {
            var productCategory = (await _productCategoryRepository.FindAsync(pc => pc.ProductId == productId && pc.CategoryId == categoryId)).FirstOrDefault();
            if (productCategory == null)
            {
                throw new NotFoundException("ProductCategory not found");
            }
            return Response<Domain.Entities.ProductCategory>.Success(productCategory);
        }

        public async Task<Response<Domain.Entities.ProductCategory>> CreateProductCategoryAsync(ProductCategoryDto productCategoryDto)
        {
            var productCategory = new Domain.Entities.ProductCategory
            {
                ProductId = productCategoryDto.ProductId,
                CategoryId = productCategoryDto.CategoryId
            };
            await _productCategoryRepository.AddAsync(productCategory);
            return Response<Domain.Entities.ProductCategory>.Success(productCategory);
        }

        public async Task<Response<bool>> DeleteProductCategoryAsync(Guid productId, Guid categoryId)
        {
            var productCategory = (await _productCategoryRepository.FindAsync(pc => pc.ProductId == productId && pc.CategoryId == categoryId)).FirstOrDefault();
            if (productCategory != null)
            {
                await _productCategoryRepository.DeleteAsync(new { productId, categoryId });
                return Response<bool>.Success(true);
            }
            throw new NotFoundException("ProductCategory not found");
        }
    }
}