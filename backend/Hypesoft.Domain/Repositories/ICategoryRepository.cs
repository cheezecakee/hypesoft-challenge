using Hypesoft.Domain.Entities;

namespace Hypesoft.Domain.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);
        Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> HasProductsAsync(string id, CancellationToken cancellationToken = default);
        Task<Dictionary<string, int>> GetAllWithProductCountAsync(CancellationToken cancellationToken = default);
        Task<Category?> GetByIdWithProductCountAsync(string id, CancellationToken cancellationToken = default);
    }
}
