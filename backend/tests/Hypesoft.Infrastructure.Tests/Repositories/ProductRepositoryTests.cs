using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.ValueObjects;
using Hypesoft.Infrastructure.Data.Context;
using Hypesoft.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Hypesoft.Infrastructure.Tests.Repositories
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new ProductRepository(_context);

            SeedData().GetAwaiter().GetResult();
        }

        private async Task SeedData()
        {
            var category = new Category("Electronics", "Electronic gadgets");

            var product1 = new Product("Laptop", "Gaming laptop", new Money(1500, "USD"), category.Id, 20);
            var product2 = new Product("Mouse", "Wireless mouse", new Money(50, "USD"), category.Id, 5);

            await _context.Categories.AddAsync(category);
            await _context.Products.AddRangeAsync(product1, product2);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Product()
        {
            var product = _context.Products.First();
            var result = await _repository.GetByIdAsync(product.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be(product.Name);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Products()
        {
            var result = await _repository.GetAllAsync();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task CreateAsync_Should_Add_Product()
        {
            var category = _context.Categories.First();
            var newProduct = new Product("Keyboard", "Mechanical keyboard", new Money(100, "USD"), category.Id, 15);

            var created = await _repository.CreateAsync(newProduct);
            created.Id.Should().NotBeNullOrEmpty();

            var allProducts = await _repository.GetAllAsync();
            allProducts.Should().HaveCount(3);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Product()
        {
            var product = _context.Products.First();
            product.Update("Laptop Pro", "High-end gaming laptop", new Money(2000, "USD"), product.CategoryId, 25);

            var updated = await _repository.UpdateAsync(product);
            updated.Name.Should().Be("Laptop Pro");
            updated.Price.Amount.Should().Be(2000);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Product()
        {
            var product = _context.Products.First();
            await _repository.DeleteAsync(product.Id);

            var exists = await _repository.ExistsAsync(product.Id);
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task GetLowStockProductsAsync_Should_Return_Products_Under_10()
        {
            var result = await _repository.GetLowStockProductsAsync();
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Mouse");
        }

        [Fact]
        public async Task SearchAsync_Should_Filter_Products()
        {
            var category = _context.Categories.First();
            var (products, total) = await _repository.SearchAsync("Laptop", category.Id, 1, 10, CancellationToken.None);

            total.Should().Be(1);
            products.First().Name.Should().Be("Laptop");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

