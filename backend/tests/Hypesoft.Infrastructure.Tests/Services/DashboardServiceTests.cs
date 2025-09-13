using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Services;
using Hypesoft.Domain.ValueObjects;
using Hypesoft.Infrastructure.Data.Context;
using Hypesoft.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Hypesoft.Infrastructure.Tests.Services
{
    public class DashboardServiceTests
    {
        private static ApplicationDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            return context;
        }

        private async Task SeedData(ApplicationDbContext context)
        {
            var category1 = new Category("Cat1", "Category 1");
            var category2 = new Category("Cat2", "Category 2");

            var product1 = new Product("Prod1", "Product 1", new Money(10), category1.Id, 5);
            var product2 = new Product("Prod2", "Product 2", new Money(20), category1.Id, 15);
            var product3 = new Product("Prod3", "Product 3", new Money(5), category2.Id, 2);

            await context.Categories.AddRangeAsync(category1, category2);
            await context.Products.AddRangeAsync(product1, product2, product3);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetTotalProductsAsync_ShouldReturnCorrectCount()
        {
            using var context = GetInMemoryDb();
            await SeedData(context);

            IDashboardService service = new DashboardService(context);
            int total = await service.GetTotalProductsAsync();

            total.Should().Be(3);
        }

        [Fact]
        public async Task GetTotalStockValueAsync_ShouldReturnCorrectValue()
        {
            using var context = GetInMemoryDb();
            await SeedData(context);

            IDashboardService service = new DashboardService(context);
            decimal totalValue = await service.GetTotalStockValueAsync();

            // Prod1: 10*5 = 50, Prod2: 20*15=300, Prod3: 5*2=10 => 50+300+10=360
            totalValue.Should().Be(360);
        }

        [Fact]
        public async Task GetLowStockProductCountAsync_ShouldReturnCorrectCount()
        {
            using var context = GetInMemoryDb();
            await SeedData(context);

            IDashboardService service = new DashboardService(context);
            int lowStockCount = await service.GetLowStockProductCountAsync();

            // Low stock < 10: Prod1(5), Prod3(2) => 2 products
            lowStockCount.Should().Be(2);
        }

        [Fact]
        public async Task GetTotalCategoriesAsync_ShouldReturnCorrectCount()
        {
            using var context = GetInMemoryDb();
            await SeedData(context);

            IDashboardService service = new DashboardService(context);
            int totalCategories = await service.GetTotalCategoriesAsync();

            totalCategories.Should().Be(2);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ShouldReturnCorrectAggregation()
        {
            using var context = GetInMemoryDb();
            await SeedData(context);

            IDashboardService service = new DashboardService(context);
            var results = (await service.GetProductsByCategoryAsync()).ToList();

            results.Should().HaveCount(2);

            var cat1 = results.First(r => r.CategoryName == "Cat1");
            cat1.ProductCount.Should().Be(2);
            cat1.TotalValue.Should().Be(350); // Prod1:50 + Prod2:300

            var cat2 = results.First(r => r.CategoryName == "Cat2");
            cat2.ProductCount.Should().Be(1);
            cat2.TotalValue.Should().Be(10); // Prod3: 5*2
        }

        [Fact]
        public async Task EmptyDatabase_ShouldReturnZeros()
        {
            using var context = GetInMemoryDb();

            IDashboardService service = new DashboardService(context);

            (await service.GetTotalProductsAsync()).Should().Be(0);
            (await service.GetTotalStockValueAsync()).Should().Be(0);
            (await service.GetLowStockProductCountAsync()).Should().Be(0);
            (await service.GetTotalCategoriesAsync()).Should().Be(0);

            var results = await service.GetProductsByCategoryAsync();
            results.Should().BeEmpty();
        }

        [Fact]
        public async Task NoLowStockProducts_ShouldReturnZeroLowStockCount()
        {
            using var context = GetInMemoryDb();

            var category = new Category("Cat1", "Category 1");
            var product = new Product("Prod1", "Product 1", new Money(10), category.Id, 20); // Stock > 10

            await context.Categories.AddAsync(category);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            IDashboardService service = new DashboardService(context);

            int lowStockCount = await service.GetLowStockProductCountAsync();
            lowStockCount.Should().Be(0);
        }

        [Fact]
        public async Task CategoryWithZeroProducts_ShouldReturnZeroInAggregation()
        {
            using var context = GetInMemoryDb();

            var category1 = new Category("Cat1", "Category 1");
            var category2 = new Category("Cat2", "Category 2"); // no products

            var product = new Product("Prod1", "Product 1", new Money(10), category1.Id, 5);

            await context.Categories.AddRangeAsync(category1, category2);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            IDashboardService service = new DashboardService(context);

            var results = (await service.GetProductsByCategoryAsync()).ToList();

            results.Should().HaveCount(2);

            var cat1 = results.First(r => r.CategoryName == "Cat1");
            cat1.ProductCount.Should().Be(1);
            cat1.TotalValue.Should().Be(50); // 10*5

            var cat2 = results.First(r => r.CategoryName == "Cat2");
            cat2.ProductCount.Should().Be(0);
            cat2.TotalValue.Should().Be(0);
        }
    }
}
