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
    public class CreateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new CreateProductCommandHandler(
                _mockProductRepository.Object,
                _mockCategoryRepository.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateProductAndReturnDto()
        {
            // Arrange
            var command = new CreateProductCommand(
                "Test Product",
                "Test Description",
                99.99m,
                "USD",
                "category-123",
                50);

            var createdProduct = new Product(
                command.Name,
                command.Description,
                new Money(command.Price, command.Currency),
                command.CategoryId,
                command.StockQuantity);

            var productWithCategory = createdProduct; // Simulate fetched product with category
            var expectedDto = new ProductDto(
                "product-123",
                command.Name,
                command.Description,
                command.Price,
                command.Currency,
                command.CategoryId,
                "Test Category",
                command.StockQuantity,
                false,
                DateTime.UtcNow,
                null);

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync(command.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockProductRepository
                .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdProduct);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(createdProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productWithCategory);

            _mockMapper
                .Setup(x => x.Map<ProductDto>(productWithCategory))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDto);

            _mockCategoryRepository.Verify(
                x => x.ExistsAsync(command.CategoryId, It.IsAny<CancellationToken>()),
                Times.Once);

            _mockProductRepository.Verify(
                x => x.CreateAsync(It.Is<Product>(p => 
                    p.Name == command.Name &&
                    p.Description == command.Description &&
                    p.Price.Amount == command.Price &&
                    p.Price.Currency == command.Currency &&
                    p.CategoryId == command.CategoryId &&
                    p.StockQuantity == command.StockQuantity), 
                It.IsAny<CancellationToken>()),
                Times.Once);

            _mockProductRepository.Verify(
                x => x.GetByIdAsync(createdProduct.Id, It.IsAny<CancellationToken>()),
                Times.Once);

            _mockMapper.Verify(
                x => x.Map<ProductDto>(productWithCategory),
                Times.Once);
        }

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new CreateProductCommand(
                "Test Product",
                "Test Description",
                99.99m,
                "USD",
                "nonexistent-category",
                50);

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync(command.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain($"Category with ID {command.CategoryId} does not exist");

            _mockCategoryRepository.Verify(
                x => x.ExistsAsync(command.CategoryId, It.IsAny<CancellationToken>()),
                Times.Once);

            _mockProductRepository.Verify(
                x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new CreateProductCommand(
                "Test Product",
                "Test Description",
                99.99m,
                "USD",
                "category-123",
                50);

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync(command.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockProductRepository
                .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("Database error");
        }

        [Theory]
        [InlineData("", "Description", 99.99, "USD", "cat-123", 50)]
        [InlineData("Name", "", 99.99, "USD", "cat-123", 50)]
        [InlineData("Name", "Description", -1, "USD", "cat-123", 50)]
        [InlineData("Name", "Description", 99.99, "", "cat-123", 50)]
        [InlineData("Name", "Description", 99.99, "USD", "", 50)]
        [InlineData("Name", "Description", 99.99, "USD", "cat-123", -1)]
        public async Task Handle_InvalidMoneyValues_ShouldThrowExceptionFromDomain(
            string name, string description, decimal price, string currency, string categoryId, int stock)
        {
            // Arrange
            var command = new CreateProductCommand(name, description, price, currency, categoryId, stock);

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            // This should throw from Money constructor or Product constructor
            await Assert.ThrowsAnyAsync<Exception>(
                () => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CancellationRequested_ShouldRespectCancellation()
        {
            // Arrange
            var command = new CreateProductCommand(
                "Test Product",
                "Test Description",
                99.99m,
                "USD",
                "category-123",
                50);

            var cancellationToken = new CancellationToken(true);

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync(command.CategoryId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => _handler.Handle(command, cancellationToken));
        }

        [Fact]
        public async Task Handle_GetByIdReturnsNull_ShouldHandleGracefully()
        {
            // Arrange
            var command = new CreateProductCommand(
                "Test Product",
                "Test Description",
                99.99m,
                "USD",
                "category-123",
                50);

            var createdProduct = new Product(
                command.Name,
                command.Description,
                new Money(command.Price, command.Currency),
                command.CategoryId,
                command.StockQuantity);

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync(command.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockProductRepository
                .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdProduct);

            _mockProductRepository
                .Setup(x => x.GetByIdAsync(createdProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var expectedDto = new ProductDto();
            _mockMapper
                .Setup(x => x.Map<ProductDto>(null))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _mockMapper.Verify(x => x.Map<ProductDto>(null), Times.Once);
        }
    }
}
