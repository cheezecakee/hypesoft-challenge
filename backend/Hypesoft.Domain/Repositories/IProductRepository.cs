using Hypesoft.Domain.Entities;

namespace Hypesoft.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(string categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default);
        Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);
        Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
        Task<Product?> GetByIdWithCategoryAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default);
        Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
            string? searchTerm,
            string? categoryId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
