using AutoMapper;
using FluentAssertions;
using Hypesoft.Application.Common.Mappings;
using Hypesoft.Application.DTOs.Products;
using Hypesoft.Application.Handlers.Products;
using Hypesoft.Application.Queries.Products;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Hypesoft.Application.Tests.Queries.Products
{
    public class GetLowStockProductsQueryHandlerTests : IDisposable
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly IMapper _mapper;
        private readonly GetLowStockProductsQueryHandler _handler;

        public GetLowStockProductsQueryHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mapper = CreateMapper();
            _handler = new GetLowStockProductsQueryHandler(_mockProductRepository.Object, _mapper);
        }

        private static IMapper CreateMapper()
        {
            using var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { });
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), loggerFactory);
            return config.CreateMapper();
        }

        private static Product CreateTestProduct(string id, string name, string categoryId, string categoryName, int stockQuantity)
        {
            var category = new Category(categoryName, $"Description for {categoryName}");
            var product = new Product(
                name,
                $"Description for {name}",
                new Money(50m, "USD"),
                categoryId,
                stockQuantity
            );

            // Set IDs using reflection if needed
            product.GetType().GetProperty("Id")!.SetValue(product, id);
            product.GetType().GetProperty("Category")!.SetValue(product, category);

            return product;
        }

        [Fact]
        public async Task Handle_ShouldReturnLowStockProducts()
        {
            // Arrange
            var lowStockProducts = new List<Product>
            {
                CreateTestProduct("1", "Product 1", "cat1", "Category 1", 2),
                CreateTestProduct("2", "Product 2", "cat2", "Category 2", 5)
            };

            _mockProductRepository
                .Setup(r => r.GetLowStockProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(lowStockProducts);

            var query = new GetLowStockProductsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(p => p.IsLowStock);
            result.First().Name.Should().Be("Product 1");
            result.Last().Name.Should().Be("Product 2");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmpty_WhenNoLowStockProducts()
        {
            // Arrange
            _mockProductRepository
                .Setup(r => r.GetLowStockProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<Product>());

            var query = new GetLowStockProductsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenCancellationRequested_ShouldThrowOperationCanceledException()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockProductRepository
                .Setup(r => r.GetLowStockProductsAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(async ct =>
                {
                    ct.ThrowIfCancellationRequested();
                    await Task.Delay(1, ct);
                    return Enumerable.Empty<Product>();
                });

            var query = new GetLowStockProductsQuery();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => _handler.Handle(query, cts.Token));
        }

        [Fact]
        public async Task Handle_ShouldMapProductsCorrectly()
        {
            // Arrange
            var product = CreateTestProduct("1", "LowStockProduct", "cat1", "Category1", 3);

            _mockProductRepository
                .Setup(r => r.GetLowStockProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { product });

            var query = new GetLowStockProductsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            var dto = result.First();

            // Assert mapping
            dto.Id.Should().Be("1");
            dto.Name.Should().Be("LowStockProduct");
            dto.CategoryId.Should().Be("cat1");
            dto.CategoryName.Should().Be("Category1");
            dto.StockQuantity.Should().Be(3);
            dto.IsLowStock.Should().BeTrue();
            dto.Price.Should().Be(50m);
            dto.Currency.Should().Be("USD");
        }

        public void Dispose()
        {
            // Clean up resources if needed
        }
    }
}
