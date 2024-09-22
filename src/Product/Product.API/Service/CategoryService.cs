using GitStartFramework.Shared.Exceptions;
using GitStartFramework.Shared.Model;
using GitStartFramework.Shared.Persistence.Repository.interfaces;
using Product.API.Model;

namespace Product.API.Service
{
    public interface ICategoryService
    {
        Task<Response<PaginatedResult<Domain.Entities.Category>>> GetAllCategoriesAsync(int pageNumber, int pageSize);

        Task<Response<Domain.Entities.Category?>> GetCategoryByIdAsync(Guid id);

        Task<Response<Domain.Entities.Category>> CreateCategoryAsync(CategoryDto category);

        Task<Response<bool>> UpdateCategoryAsync(UpdateCategoryDto category);

        Task<Response<bool>> DeleteCategoryAsync(Guid id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Domain.Entities.Category> _categoryRepository;

        public CategoryService(IGenericRepository<Domain.Entities.Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Response<PaginatedResult<Domain.Entities.Category>>> GetAllCategoriesAsync(int pageNumber, int pageSize)
        {
            var totalItems = (await _categoryRepository.GetAllAsync()).Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var categories = (await _categoryRepository.GetAllAsync())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Response<PaginatedResult<Domain.Entities.Category>>.Success(new PaginatedResult<Domain.Entities.Category>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = categories
            });
        }

        public async Task<Response<Domain.Entities.Category?>> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null)
            {
                throw new NotFoundException("Category not found");
            }
            return Response<Domain.Entities.Category>.Success(category);
        }

        public async Task<Response<Domain.Entities.Category>> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = new Domain.Entities.Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                IsActive = true
            };
            await _categoryRepository.AddAsync(category);
            return Response<Domain.Entities.Category>.Success(category);
        }

        public async Task<Response<bool>> UpdateCategoryAsync(UpdateCategoryDto categoryToUpdate)
        {
            var category = await GetCategoryByIdAsync(categoryToUpdate.Id.Value);
            category.Data.Name = categoryToUpdate.Name ?? category.Data.Name;
            category.Data.Description = categoryToUpdate.Description ?? category.Data.Description;
            await _categoryRepository.UpdateAsync(category.Data);
            return Response<bool>.Success(true);
        }

        public async Task<Response<bool>> DeleteCategoryAsync(Guid id)
        {
            await _categoryRepository.DeleteAsync(id);
            return Response<bool>.Success(true);
        }
    }
}