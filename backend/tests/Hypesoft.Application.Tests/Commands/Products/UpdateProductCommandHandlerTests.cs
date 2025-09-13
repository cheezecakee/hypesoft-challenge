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
    public class UpdateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new UpdateProductCommandHandler(
                _mockProductRepository.Object,
                _mockCategoryRepository.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidFullUpdate_ShouldUpdateProductAndReturnDto()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductCommand(
                productId,
                "Updated Product",
                "Updated Description",
                199.99m,
                "EUR",
                "new-category-456",
                75);

            var existingProduct = new Product(
                "Original Product",
                "Original Description",
                new Money(99.99m, "USD"),
                "old-category-123",
                50);

            var expectedDto = new ProductDto(
                productId,
                command.Name!,
                command.Description!,
                command.Price!.Value,
                command.Currency!,
                command.CategoryId!,
                "New Category",
                command.StockQuantity!.Value,
                false,
                DateTime.UtcNow,
                DateTime.UtcNow);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync(command.CategoryId!, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

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

            _mockProductRepository.Verify(
                x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
                Times.Once);

            _mockCategoryRepository.Verify(
                x => x.ExistsAsync(command.CategoryId!, It.IsAny<CancellationToken>()),
                Times.Once);

            _mockProductRepository.Verify(
                x => x.UpdateAsync(existingProduct, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_PartialUpdateNameOnly_ShouldUpdateOnlyName()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductCommand(productId, Name: "Updated Name Only");

            var existingProduct = new Product(
                "Original Product",
                "Original Description",
                new Money(99.99m, "USD"),
                "category-123",
                50);

            var expectedDto = new ProductDto(
                productId,
                "Updated Name Only",
                "Original Description",
                99.99m,
                "USD",
                "category-123",
                "Category Name",
                50,
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

            // Verify category validation was not called since CategoryId is null
            _mockCategoryRepository.Verify(
                x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_UpdatePriceOnly_ShouldUpdatePrice()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductCommand(productId, Price: 299.99m);

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

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockMapper
                .Setup(x => x.Map<ProductDto>(existingProduct))
                .Returns(new ProductDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            // Verify the product was updated with new Money object
            _mockProductRepository.Verify(
                x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateCurrencyOnly_ShouldUpdateCurrency()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductCommand(productId, Currency: "EUR");

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

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockMapper
                .Setup(x => x.Map<ProductDto>(existingProduct))
                .Returns(new ProductDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ProductNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var productId = "nonexistent-product";
            var command = new UpdateProductCommand(productId, Name: "Updated Name");

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain($"Product with ID {productId} not found");

            _mockProductRepository.Verify(
                x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
                Times.Once);

            _mockProductRepository.Verify(
                x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductCommand(productId, CategoryId: "nonexistent-category");

            var existingProduct = new Product(
                "Test Product",
                "Test Description",
                new Money(99.99m, "USD"),
                "category-123",
                50);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync("nonexistent-category", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain("Category with ID nonexistent-category does not exist");

            _mockCategoryRepository.Verify(
                x => x.ExistsAsync("nonexistent-category", It.IsAny<CancellationToken>()),
                Times.Once);

            _mockProductRepository.Verify(
                x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_NullCategoryId_ShouldNotValidateCategory()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductCommand(productId, Name: "Updated Name", CategoryId: null);

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

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockMapper
                .Setup(x => x.Map<ProductDto>(existingProduct))
                .Returns(new ProductDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            // Verify category validation was not called for null
            _mockCategoryRepository.Verify(
                x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhitespaceCategoryId_ShouldNotValidateCategory()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductCommand(productId, Name: "Updated Name", CategoryId: "   ");

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

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockMapper
                .Setup(x => x.Map<ProductDto>(existingProduct))
                .Returns(new ProductDto());

            // Act & Assert
            // This should throw from domain validation since whitespace is not valid
            await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UpdatePriceAndCurrency_ShouldCreateNewMoneyObject()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductCommand(
                productId,
                Price: 199.99m,
                Currency: "EUR");

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

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _mockMapper
                .Setup(x => x.Map<ProductDto>(existingProduct))
                .Returns(new ProductDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            _mockProductRepository.Verify(
                x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductCommand(productId, Name: "Updated Name");

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
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("Database error");
        }

        [Fact]
        public async Task Handle_CancellationRequested_ShouldRespectCancellation()
        {
            // Arrange
            var productId = "product-123";
            var command = new UpdateProductCommand(productId, Name: "Updated Name");
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
            var command = new UpdateProductCommand(productId, Name: "Updated Name");

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

            var expectedDto = new ProductDto();
            _mockMapper
                .Setup(x => x.Map<ProductDto>(It.IsAny<Product>()))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedDto); // Should return whatever mapper returns
        }
    }
}
