using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Hypesoft.Domain.Entities;
using Hypesoft.Infrastructure.Data.Context;
using Hypesoft.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Hypesoft.Infrastructure.Tests.Repositories
{
    public class CategoryRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CategoryRepository _repository;

        public CategoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new CategoryRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task CreateAsync_Should_Add_Category()
        {
            var category = new Category("Electronics", "Electronic devices");

            var result = await _repository.CreateAsync(category);

            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            _context.Categories.Should().ContainSingle(c => c.Id == result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Category()
        {
            var category = new Category("Books", "All kinds of books");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(category.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Books");
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Categories()
        {
            var cat1 = new Category("Cat1", "Desc1");
            var cat2 = new Category("Cat2", "Desc2");
            await _context.Categories.AddRangeAsync(cat1, cat2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            result.Should().HaveCount(2);
            result.Select(c => c.Name).Should().Contain(new[] { "Cat1", "Cat2" });
        }

        [Fact]
        public async Task GetByNameAsync_Should_Return_Category()
        {
            var category = new Category("Sports", "Sporting goods");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByNameAsync("Sports");

            result.Should().NotBeNull();
            result!.Description.Should().Be("Sporting goods");
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Category()
        {
            var category = new Category("OldName", "OldDesc");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            category.Update("NewName", "NewDesc");
            var result = await _repository.UpdateAsync(category);

            result.Name.Should().Be("NewName");
            result.Description.Should().Be("NewDesc");
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Category()
        {
            var category = new Category("ToDelete", "Desc");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(category.Id);

            var exists = await _repository.ExistsAsync(category.Id);
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_Should_Return_True_If_Category_Exists()
        {
            var category = new Category("Exists", "Desc");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await _repository.ExistsAsync(category.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasProductsAsync_Should_Return_True_If_Category_Has_Products()
        {
            var category = new Category("WithProducts", "Desc");
            await _context.Categories.AddAsync(category);
            var product = new Product("Product1", "Desc", new Domain.ValueObjects.Money(100, "USD"), category.Id, 5);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var result = await _repository.HasProductsAsync(category.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllWithProductCountAsync_Should_Return_Categories_With_Products_Only()
        {
            var cat1 = new Category("CatA", "DescA");
            var cat2 = new Category("CatB", "DescB");
            await _context.Categories.AddRangeAsync(cat1, cat2);
            
            // Add products only to cat1
            var product1 = new Product("Product1", "Desc1", new Domain.ValueObjects.Money(100, "USD"), cat1.Id, 5);
            var product2 = new Product("Product2", "Desc2", new Domain.ValueObjects.Money(200, "USD"), cat1.Id, 3);
            await _context.Products.AddRangeAsync(product1, product2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllWithProductCountAsync();

            // Should only return cat1 (which has products), not cat2 (which has no products)
            result.Should().HaveCount(1);
            result.Should().ContainKey(cat1.Id);
            result[cat1.Id].Should().Be(2); // cat1 has 2 products
        }

        [Fact]
        public async Task GetAllWithProductCountAsync_Should_Return_Empty_When_No_Products()
        {
            // Test for when no products exist at all
            var cat1 = new Category("CatA", "DescA");
            var cat2 = new Category("CatB", "DescB");
            await _context.Categories.AddRangeAsync(cat1, cat2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllWithProductCountAsync();

            result.Should().BeEmpty(); // No categories should be returned since none have products
        }

        [Fact]
        public async Task GetByIdWithProductCountAsync_Should_Return_Category()
        {
            var category = new Category("CatX", "DescX");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdWithProductCountAsync(category.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("CatX");
        }
    }
}
