using Microsoft.EntityFrameworkCore;
using Hypesoft.Infrastructure.Data.Context;
using Hypesoft.Domain.Entities;

namespace Hypesoft.Infrastructure.Services
{
    public class DashboardService(ApplicationDbContext context) : IDashboardService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products.CountAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default)
        {
            List<Product> products = await _context.Products.ToListAsync(cancellationToken);
            return products.Sum(static p => p.Price.Amount * p.StockQuantity);
        }

        public async Task<IEnumerable<object>> GetProductsByCategory(CancellationToken cancellationToken = default)
        {
            var productsWithCategories = await (
                from p in _context.Products
                join c in _context.Categories on p.CategoryId equals c.Id
                group p by new { c.Id, c.Name } into g
                select new
                {
                    CategoryId = g.Key.Id,
                    CategoryName = g.Key.Name,
                    ProductCount = g.Count()
                }
            ).ToListAsync(cancellationToken);

            return productsWithCategories;
        }
    }

    public interface IDashboardService
    {
        Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default);
        Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<object>> GetProductsByCategory(CancellationToken cancellationToken = default);
    }
}
