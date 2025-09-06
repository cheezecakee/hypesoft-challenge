using Microsoft.EntityFrameworkCore;
using Hypesoft.Domain.Services;
using Hypesoft.Infrastructure.Data.Context;

namespace Hypesoft.Infrastructure.Services
{
    public class DashboardService(ApplicationDbContext context) : IDashboardService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products.CountAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default)
        {
            // Fetch all products first, then calculate in memory
            var products = await _context.Products.ToListAsync(cancellationToken);
            return products.Sum(p => p.Price.Amount * p.StockQuantity);
        }

        public async Task<int> GetLowStockProductCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .CountAsync(p => p.StockQuantity < 10, cancellationToken);
        }

        public async Task<int> GetTotalCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories.CountAsync(cancellationToken);
        }

        public async Task<IEnumerable<(string CategoryId, string CategoryName, int ProductCount, decimal TotalValue)>> GetProductsByCategoryAsync(CancellationToken cancellationToken = default)
        {
            // Get all data first, then process in memory
            var categories = await _context.Categories.ToListAsync(cancellationToken);
            var products = await _context.Products.ToListAsync(cancellationToken);

            // Group products by category in memory
            var productsByCategory = products
                .GroupBy(p => p.CategoryId)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        ProductCount = g.Count(),
                        TotalValue = g.Sum(p => p.Price.Amount * p.StockQuantity)
                    }
                );

            // Combine with category data
            var result = categories.Select(category =>
            {
                var hasProducts = productsByCategory.TryGetValue(category.Id, out var productData);
                return (
                    CategoryId: category.Id,
                    CategoryName: category.Name,
                    ProductCount: hasProducts ? productData.ProductCount : 0,
                    TotalValue: hasProducts ? productData.TotalValue : 0m
                );
            });

            return result;
        }
    }
}
