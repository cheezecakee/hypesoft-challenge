using FluentAssertions;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.ValueObjects;
using Hypesoft.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ApplicationDbContextTests
{
    private ApplicationDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CanAddAndRetrieveProduct()
    {
        using var context = GetInMemoryDb();

        var category = new Category("Cat1", "Category 1");
        await context.Categories.AddAsync(category);

        var product = new Product("Prod1", "Product 1", new Money(10), category.Id, 5);
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var retrieved = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == product.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Category.Id.Should().Be(category.Id);
    }

    [Fact]
    public void CreatingProduct_WithEmptyCategoryId_ShouldThrow()
    {
        var money = new Money(10);

        Action act = () => new Product("Name", "Desc", money, "", 5);

        act.Should().Throw<ArgumentException>()
           .WithMessage("Category ID is required*");
    }

    [Fact]
    public void CreatingProduct_WithEmptyName_ShouldThrow()
    {
        var money = new Money(10);
        var categoryId = Guid.NewGuid().ToString();

        Action act = () => new Product("", "Desc", money, categoryId, 5);

        act.Should().Throw<ArgumentException>()
           .WithMessage("Product name is required*");
    }

    [Fact]
    public void CreatingProduct_WithNegativeStock_ShouldThrow()
    {
        var money = new Money(10);
        var categoryId = Guid.NewGuid().ToString();

        Action act = () => new Product("Name", "Desc", money, categoryId, -1);

        act.Should().Throw<ArgumentException>()
           .WithMessage("Stock quantity cannot be negative*");
    }
}
