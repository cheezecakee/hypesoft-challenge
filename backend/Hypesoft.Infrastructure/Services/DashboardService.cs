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
            return await _context.Products
                .SumAsync(static p => p.Price.Amount * p.StockQuantity, cancellationToken);
        }

        public async Task<int> GetLowStockProductCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .CountAsync(static p => p.StockQuantity < 10, cancellationToken);
        }

        public async Task<int> GetTotalCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories.CountAsync(cancellationToken);
        }

        public async Task<IEnumerable<(string CategoryId, string CategoryName, int ProductCount, decimal TotalValue)>> GetProductsByCategoryAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Select(c => new
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id),
                    TotalValue = _context.Products
                        .Where(p => p.CategoryId == c.Id)
                        .Sum(p => p.Price.Amount * p.StockQuantity)
                })
                .Select(x => ValueTuple.Create(x.CategoryId, x.CategoryName, x.ProductCount, x.TotalValue))
                .ToListAsync(cancellationToken);
        }
    }
}
