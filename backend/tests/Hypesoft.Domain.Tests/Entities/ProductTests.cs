using Hypesoft.Domain.Entities;
using Hypesoft.Domain.ValueObjects;

namespace Hypesoft.Domain.Tests.Entities
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_ValidValues_ShouldCreateProduct()
        {
            // Arrange
            var name = "Test Product";
            var description = "Test Description";
            var price = new Money(99.99m, "USD");
            var categoryId = "category-123";
            var stockQuantity = 50;

            // Act
            var product = new Product(name, description, price, categoryId, stockQuantity);

            // Assert
            product.Name.Should().Be(name);
            product.Description.Should().Be(description);
            product.Price.Should().Be(price);
            product.CategoryId.Should().Be(categoryId);
            product.StockQuantity.Should().Be(stockQuantity);
            product.IsLowStock.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_EmptyOrNullName_ShouldThrowException(string invalidName)
        {
            // Arrange
            var description = "Test Description";
            var price = new Money(99.99m, "USD");
            var categoryId = "category-123";
            var stockQuantity = 50;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Product(invalidName, description, price, categoryId, stockQuantity));
            exception.Message.Should().Contain("Product name is required");
        }

        [Fact]
        public void Constructor_NameTooLong_ShouldThrowException()
        {
            // Arrange
            var longName = new string('a', 201); // 201 characters
            var description = "Test Description";
            var price = new Money(99.99m, "USD");
            var categoryId = "category-123";
            var stockQuantity = 50;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Product(longName, description, price, categoryId, stockQuantity));
            exception.Message.Should().Contain("Product name cannot exceed 200 characters");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_EmptyOrNullDescription_ShouldThrowException(string invalidDescription)
        {
            // Arrange
            var name = "Test Product";
            var price = new Money(99.99m, "USD");
            var categoryId = "category-123";
            var stockQuantity = 50;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Product(name, invalidDescription, price, categoryId, stockQuantity));
            exception.Message.Should().Contain("Product description is required");
        }

        [Fact]
        public void Constructor_NullPrice_ShouldThrowException()
        {
            // Arrange
            var name = "Test Product";
            var description = "Test Description";
            Money nullPrice = null!;
            var categoryId = "category-123";
            var stockQuantity = 50;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Product(name, description, nullPrice, categoryId, stockQuantity));
        }

        [Fact]
        public void Constructor_NegativeStock_ShouldThrowException()
        {
            // Arrange
            var name = "Test Product";
            var description = "Test Description";
            var price = new Money(99.99m, "USD");
            var categoryId = "category-123";
            var negativeStock = -1;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Product(name, description, price, categoryId, negativeStock));
            exception.Message.Should().Contain("Stock quantity cannot be negative");
        }

        [Theory]
        [InlineData(5, true)]
        [InlineData(9, true)]
        [InlineData(10, false)]
        [InlineData(15, false)]
        public void IsLowStock_VariousStockLevels_ShouldReturnExpectedResult(int stockQuantity, bool expectedLowStock)
        {
            // Arrange
            var product = new Product("Test Product", "Description", new Money(99.99m), "cat-1", stockQuantity);

            // Act & Assert
            product.IsLowStock.Should().Be(expectedLowStock);
        }

        [Fact]
        public void UpdateStock_ValidQuantity_ShouldUpdateStock()
        {
            // Arrange
            var product = new Product("Test Product", "Description", new Money(99.99m), "cat-1", 50);
            var originalUpdatedAt = product.UpdatedAt;

            // Act
            product.UpdateStock(75);

            // Assert
            product.StockQuantity.Should().Be(75);
            product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }

        [Fact]
        public void UpdateStock_NegativeQuantity_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Test Product", "Description", new Money(99.99m), "cat-1", 50);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => product.UpdateStock(-5));
            exception.Message.Should().Contain("Stock quantity cannot be negative");
        }

        [Fact]
        public void AddStock_ValidQuantity_ShouldIncreaseStock()
        {
            // Arrange
            var product = new Product("Test Product", "Description", new Money(99.99m), "cat-1", 50);
            var originalUpdatedAt = product.UpdatedAt;

            // Act
            product.AddStock(25);

            // Assert
            product.StockQuantity.Should().Be(75);
            product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void AddStock_ZeroOrNegativeQuantity_ShouldThrowException(int invalidQuantity)
        {
            // Arrange
            var product = new Product("Test Product", "Description", new Money(99.99m), "cat-1", 50);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => product.AddStock(invalidQuantity));
            exception.Message.Should().Contain("Quantity to add must be positive");
        }

        [Fact]
        public void RemoveStock_ValidQuantity_ShouldDecreaseStock()
        {
            // Arrange
            var product = new Product("Test Product", "Description", new Money(99.99m), "cat-1", 50);
            var originalUpdatedAt = product.UpdatedAt;

            // Act
            product.RemoveStock(20);

            // Assert
            product.StockQuantity.Should().Be(30);
            product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }

        [Fact]
        public void RemoveStock_InsufficientStock_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Test Product", "Description", new Money(99.99m), "cat-1", 10);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => product.RemoveStock(15));
            exception.Message.Should().Contain("Insufficient stock");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void RemoveStock_ZeroOrNegativeQuantity_ShouldThrowException(int invalidQuantity)
        {
            // Arrange
            var product = new Product("Test Product", "Description", new Money(99.99m), "cat-1", 50);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => product.RemoveStock(invalidQuantity));
            exception.Message.Should().Contain("Quantity to remove must be positive");
        }

        [Fact]
        public void PartialUpdate_OnlyName_ShouldUpdateOnlyName()
        {
            // Arrange
            var product = new Product("Old Name", "Old Description", new Money(99.99m), "cat-1", 50);
            var originalDescription = product.Description;
            var originalPrice = product.Price;

            // Act
            product.PartialUpdate(name: "New Name");

            // Assert
            product.Name.Should().Be("New Name");
            product.Description.Should().Be(originalDescription);
            product.Price.Should().Be(originalPrice);
        }

        [Fact]
        public void PartialUpdate_NoChanges_ShouldNotUpdateTimestamp()
        {
            // Arrange
            var product = new Product("Test Product", "Description", new Money(99.99m), "cat-1", 50);
            var originalUpdatedAt = product.UpdatedAt;

            // Act
            product.PartialUpdate(name: "Test Product"); // Same name

            // Assert
            product.UpdatedAt.Should().Be(originalUpdatedAt);
        }
    }
}

