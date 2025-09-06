using Microsoft.EntityFrameworkCore;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Infrastructure.Data.Context;

namespace Hypesoft.Infrastructure.Repositories
{
    public class ProductRepository(ApplicationDbContext context) : IProductRepository
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.Name.Contains(name))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default)
        {
            // Fetch products first, then load categories separately
            var lowStockProducts = await _context.Products
                .Where(p => p.StockQuantity < 10)
                .ToListAsync(cancellationToken);

            // Load categories for these products
            var categoryIds = lowStockProducts.Select(p => p.CategoryId).Distinct().ToList();
            var categories = await _context.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            // Set navigation properties manually
            foreach (var product in lowStockProducts)
            {
                product.GetType().GetProperty("Category")?.SetValue(
                    product,
                    categories.FirstOrDefault(c => c.Id == product.CategoryId)
                );
            }

            return lowStockProducts;
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            int totalCount = await _context.Products.CountAsync(cancellationToken);
            List<Product> products = await _context.Products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (products, totalCount);
        }

        public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return product;
        }

        public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync(cancellationToken);
            return product;
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            Product? product = await GetByIdAsync(id, cancellationToken);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AnyAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
            string? searchTerm,
            string? categoryId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Product> query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Get total count without ordering
            int totalCount = await query.CountAsync(cancellationToken);

            // Get products and convert to list first, then sort in memory
            var allProducts = await query.ToListAsync(cancellationToken);
            var sortedProducts = allProducts
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Load categories for the paginated products
            var categoryIds = sortedProducts.Select(p => p.CategoryId).Distinct().ToList();
            var categories = await _context.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            foreach (var product in sortedProducts)
            {
                product.GetType().GetProperty("Category")?.SetValue(
                    product,
                    categories.FirstOrDefault(c => c.Id == product.CategoryId)
                );
            }

            return (sortedProducts, totalCount);
        }

        public async Task<Product?> GetByIdWithCategoryAsync(string id, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (product != null)
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == product.CategoryId, cancellationToken);
                product.GetType().GetProperty("Category")?.SetValue(product, category);
            }

            return product;
        }

        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default)
        {
            var products = await _context.Products.ToListAsync(cancellationToken);
            var categories = await _context.Categories.ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                product.GetType().GetProperty("Category")?.SetValue(
                    product,
                    categories.FirstOrDefault(c => c.Id == product.CategoryId)
                );
            }

            return products;
        }
    }
}
