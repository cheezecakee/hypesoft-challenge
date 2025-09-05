namespace Hypesoft.Domain.Services
{
    public interface IDashboardService
    {
        Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default);
        Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default);
        Task<int> GetLowStockProductCountAsync(CancellationToken cancellationToken = default);
        Task<int> GetTotalCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<(string CategoryId, string CategoryName, int ProductCount, decimal TotalValue)>> GetProductsByCategoryAsync(CancellationToken cancellationToken = default);
    }
}