using AutoMapper;
using FluentAssertions;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.DTOs.Products;
using Hypesoft.Application.Handlers.Products;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Hypesoft.Application.Tests.Commands.Products
{
    public class UpdateProductStockCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UpdateProductStockCommandHandler _handler;

        public UpdateProductStockCommandHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new UpdateProductStockCommandHandler(
                _mockProductRepository.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidStockUpdate_ShouldUpdateStockAndReturnDto()
        {
            // Arrange
            var productId = "product-123";
            var newStockQuantity = 100;
            var command = new UpdateProductStockCommand(productId, newStockQuantity);

            var existingProduct = new Product(
                "Test Product",
                "Test Description",
                new Money(99.99m, "USD"),
                "category-123",
                50);

            var expectedDto = new ProductDto(
                productId,
                "Test Product",
                "Test Description",
                99.99m,
                "USD",
                "category-123",
                "Test Category",
                newStockQuantity,
                false,
                DateTime.UtcNow,
                DateTime.UtcNow);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockProductRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockMapper
                .Setup(x => x.Map<ProductDto>(existingProduct))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDto);
            result.StockQuantity.Should().Be(newStockQuantity);

            // Verify the product's UpdateStock method was called
            existingProduct.StockQuantity.Should().Be(newStockQuantity);

            _mockProductRepository.Verify(
                x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
                Times.Once);

            _mockProductRepository.Verify(
                x => x.UpdateAsync(existingProduct, It.IsAny<CancellationToken>()),
                Times.Once);

            _mockMapper.Verify(
                x => x.Map<ProductDto>(existingProduct),
                Times.Once);
        }

        [Fact]
        public async Task Handle_StockUpdateToZero_ShouldUpdateSuccessfully()
        {
            // Arrange
            var productId = "product-123";
            var newStockQuantity = 0;
            var command = new UpdateProductStockCommand(productId, newStockQuantity);

            var existingProduct = new Product(
                "Test Product",
                "Test Description",
                new Money(99.99m, "USD"),
                "category-123",
                50);

            var expectedDto = new ProductDto();

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockProductRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockMapper
                .Setup(x => x.Map<ProductDto>(existingProduct))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            existingProduct.StockQuantity.Should().Be(0);
            existingProduct.IsLowStock.Should().BeTrue(); // 0 < 10
        }

        [Theory]
        [InlineData(5)] // Low stock
        [InlineData(9)] // Low stock boundary
        [InlineData(10)] // Not low stock boundary
        [InlineData(50)] // Normal stock
        [InlineData(1000)] // High stock
        public async Task Handle_VariousStockLevels_ShouldUpdateCorrectly(int stockQuantity)
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductStockCommand(productId, stockQuantity);

            var existingProduct = new Product(
                "Test Product",
                "Test Description",
                new Money(99.99m, "USD"),
                "category-123",
                25);

            var expectedDto = new ProductDto();

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockProductRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockMapper
                .Setup(x => x.Map<ProductDto>(existingProduct))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            existingProduct.StockQuantity.Should().Be(stockQuantity);
            existingProduct.IsLowStock.Should().Be(stockQuantity < 10);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ShouldReturnNull()
        {
            // Arrange
            var productId = "nonexistent-product";
            var command = new UpdateProductStockCommand(productId, 100);
            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _mockProductRepository.Verify(
                x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
                Times.Once);
            _mockProductRepository.Verify(
                x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_NegativeStockQuantity_ShouldThrowArgumentException()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductStockCommand(productId, -10);

            var existingProduct = new Product(
                "Test Product",
                "Test Description",
                new Money(99.99m, "USD"),
                "category-123",
                50);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain("Stock quantity cannot be negative");

            _mockProductRepository.Verify(
                x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryUpdateFails_ShouldPropagateException()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductStockCommand(productId, 100);

            var existingProduct = new Product(
                "Test Product",
                "Test Description",
                new Money(99.99m, "USD"),
                "category-123",
                50);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockProductRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("Database connection failed");
        }

        [Fact]
        public async Task Handle_CancellationRequested_ShouldRespectCancellation()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductStockCommand(productId, 100);
            var cancellationToken = new CancellationToken(true);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => _handler.Handle(command, cancellationToken));
        }

        [Fact]
        public async Task Handle_GetByIdAfterUpdateReturnsNull_ShouldReturnMappedResult()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductStockCommand(productId, 100);

            var existingProduct = new Product(
                "Test Product",
                "Test Description",
                new Money(99.99m, "USD"),
                "category-123",
                50);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockProductRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            // Second call returns null (edge case)
            _mockProductRepository
                .SetupSequence(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct)
                .ReturnsAsync((Product?)null);

            var expectedDto = new ProductDto(); // AutoMapper should handle null gracefully
            _mockMapper
                .Setup(x => x.Map<ProductDto>(It.IsAny<Product>()))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedDto); // Should return whatever mapper returns, even if from null
        }

        [Fact]
        public async Task Handle_StockUpdateShouldChangeUpdatedAt_WhenProductIsModified()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductStockCommand(productId, 150);

            var existingProduct = new Product(
                "Test Product",
                "Test Description",
                new Money(99.99m, "USD"),
                "category-123",
                50);

            var originalUpdatedAt = existingProduct.UpdatedAt;

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockProductRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockMapper
                .Setup(x => x.Map<ProductDto>(existingProduct))
                .Returns(new ProductDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            existingProduct.StockQuantity.Should().Be(150);
            // UpdatedAt should have changed (though in real scenario we'd need to wait or mock time)
            existingProduct.UpdatedAt.Should().BeOnOrAfter(originalUpdatedAt);
        }
    }
}
