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
    public class GetProductsQueryHandlerTests : IDisposable
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly IMapper _mapper;
        private readonly GetProductsQueryHandler _handler;

        public GetProductsQueryHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mapper = CreateMapper();
            _handler = new GetProductsQueryHandler(_mockProductRepository.Object, _mapper);
        }

        private static IMapper CreateMapper()
        {
            // AutoMapper 15.0+ requires ILoggerFactory
            using var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { });
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), loggerFactory);
            return config.CreateMapper();
        }

        private static Product CreateTestProduct(
            string id,
            string name,
            string categoryId,
            string categoryName,
            decimal price = 99.99m,
            int stockQuantity = 10)
        {
            // Create category
            var category = new Category(categoryName, $"Description for {categoryName}");
            SetId(category, categoryId);

            // Create product
            var product = new Product(
                name,
                $"Description for {name}",
                new Money(price, "USD"),
                categoryId,
                stockQuantity
            );

            SetId(product, id);
            SetCategory(product, category);

            return product;
        }

        private static void SetId(object entity, string id)
        {
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null && idProperty.CanWrite)
            {
                idProperty.SetValue(entity, id);
            }
        }

        private static void SetCategory(Product product, Category category)
        {
            var categoryProperty = product.GetType().GetProperty("Category");
            if (categoryProperty != null && categoryProperty.CanWrite)
            {
                categoryProperty.SetValue(product, category);
            }
        }

        [Fact]
        public async Task Handle_WithBasicPagination_ShouldReturnPagedResults()
        {
            // Arrange
            var products = new List<Product>
            {
                CreateTestProduct("1", "Laptop", "electronics", "Electronics", 999.99m, 15),
                CreateTestProduct("2", "Mouse", "electronics", "Electronics", 29.99m, 50),
                CreateTestProduct("3", "Keyboard", "electronics", "Electronics", 79.99m, 25)
            };

            _mockProductRepository
                .Setup(r => r.GetPagedAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync((products.Take(2), 3));

            var query = new GetProductsQuery(Page: 1, PageSize: 2);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Products.Should().HaveCount(2);
            result.TotalCount.Should().Be(3);
            
            var productList = result.Products.ToList();
            productList[0].Name.Should().Be("Laptop");
            productList[1].Name.Should().Be("Mouse");
        }

        [Fact]
        public async Task Handle_WithSearchName_ShouldSearchAndPaginate()
        {
            // Arrange
            var searchResults = new List<Product>
            {
                CreateTestProduct("1", "Gaming Laptop", "electronics", "Electronics"),
                CreateTestProduct("2", "Business Laptop", "electronics", "Electronics"),
                CreateTestProduct("3", "Student Laptop", "electronics", "Electronics")
            };

            _mockProductRepository
                .Setup(r => r.SearchByNameAsync("Laptop", It.IsAny<CancellationToken>()))
                .ReturnsAsync(searchResults);

            var query = new GetProductsQuery(Page: 1, PageSize: 2, SearchName: "Laptop");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Products.Should().HaveCount(2); // Paginated from 3 results
            result.TotalCount.Should().Be(3); // Total count from search
            
            var productList = result.Products.ToList();
            productList[0].Name.Should().Be("Gaming Laptop");
            productList[1].Name.Should().Be("Business Laptop");

            // Verify search was called, not GetPagedAsync
            _mockProductRepository.Verify(
                r => r.SearchByNameAsync("Laptop", It.IsAny<CancellationToken>()), 
                Times.Once);
            _mockProductRepository.Verify(
                r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), 
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithCategoryId_ShouldFilterByCategoryAndPaginate()
        {
            // Arrange
            var categoryProducts = new List<Product>
            {
                CreateTestProduct("1", "iPhone", "phones", "Phones"),
                CreateTestProduct("2", "Samsung", "phones", "Phones"),
                CreateTestProduct("3", "Pixel", "phones", "Phones"),
                CreateTestProduct("4", "OnePlus", "phones", "Phones")
            };

            _mockProductRepository
                .Setup(r => r.GetByCategoryIdAsync("phones", It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoryProducts);

            var query = new GetProductsQuery(Page: 2, PageSize: 2, CategoryId: "phones");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Products.Should().HaveCount(2); // Second page with 2 items
            result.TotalCount.Should().Be(4); // Total in category
            
            var productList = result.Products.ToList();
            productList[0].Name.Should().Be("Pixel"); // Skip 2, take 2
            productList[1].Name.Should().Be("OnePlus");

            // Verify category filter was called
            _mockProductRepository.Verify(
                r => r.GetByCategoryIdAsync("phones", It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithBothSearchAndCategory_ShouldPrioritizeSearch()
        {
            // Arrange - Search should take priority over category filtering
            var searchResults = new List<Product>
            {
                CreateTestProduct("1", "Gaming Chair", "furniture", "Furniture"), // Different category
                CreateTestProduct("2", "Gaming Desk", "furniture", "Furniture")
            };

            _mockProductRepository
                .Setup(r => r.SearchByNameAsync("Gaming", It.IsAny<CancellationToken>()))
                .ReturnsAsync(searchResults);

            var query = new GetProductsQuery(
                Page: 1, 
                PageSize: 10, 
                SearchName: "Gaming", 
                CategoryId: "electronics"); // Should be ignored

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Products.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            
            // Verify search was called, not category filter
            _mockProductRepository.Verify(
                r => r.SearchByNameAsync("Gaming", It.IsAny<CancellationToken>()), 
                Times.Once);
            _mockProductRepository.Verify(
                r => r.GetByCategoryIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), 
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithEmptySearch_ShouldReturnEmpty()
        {
            // Arrange
            _mockProductRepository
                .Setup(r => r.SearchByNameAsync("NonExistentProduct", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            var query = new GetProductsQuery(Page: 1, PageSize: 10, SearchName: "NonExistentProduct");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Products.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Handle_WithLastPage_ShouldReturnRemainingItems()
        {
            // Arrange
            var allProducts = Enumerable.Range(1, 7)
                .Select(i => CreateTestProduct(i.ToString(), $"Product {i}", "cat1", "Category"))
                .ToList();

            _mockProductRepository
                .Setup(r => r.GetPagedAsync(3, 3, It.IsAny<CancellationToken>()))
                .ReturnsAsync((allProducts.Skip(6).Take(3), 7)); // Last page with 1 item

            var query = new GetProductsQuery(Page: 3, PageSize: 3);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Products.Should().HaveCount(1); // Only 1 item on last page
            result.TotalCount.Should().Be(7);
            result.Products.First().Name.Should().Be("Product 7");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Handle_WithEmptyOrWhitespaceSearch_ShouldUseBasicPagination(string? searchTerm)
        {
            // Arrange
            var products = new List<Product>
            {
                CreateTestProduct("1", "Product 1", "cat1", "Category")
            };

            _mockProductRepository
                .Setup(r => r.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((products, 1));

            var query = new GetProductsQuery(Page: 1, PageSize: 10, SearchName: searchTerm);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Products.Should().HaveCount(1);
            
            // Should use GetPagedAsync, not search
            _mockProductRepository.Verify(
                r => r.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()), 
                Times.Once);
            _mockProductRepository.Verify(
                r => r.SearchByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), 
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrows_ShouldPropagateException()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Database error");
            
            _mockProductRepository
                .Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            var query = new GetProductsQuery(Page: 1, PageSize: 10);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
            
            exception.Message.Should().Be("Database error");
        }

        [Fact]
        public async Task Handle_WhenCancellationRequested_ShouldThrowOperationCanceledException()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            cts.Cancel(); // Cancel immediately

            var query = new GetProductsQuery(Page: 1, PageSize: 10);

            _mockProductRepository
                .Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns<int, int, CancellationToken>(async (page, pageSize, ct) =>
                {
                    ct.ThrowIfCancellationRequested();
                    await Task.Delay(1, ct); // This should throw
                    return (new List<Product>(), 0);
                });

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _handler.Handle(query, cts.Token));
        }

        [Fact]
        public async Task Handle_ShouldMapProductsToDto_WithCorrectProperties()
        {
            // Arrange
            var product = CreateTestProduct(
                id: "test-id",
                name: "Test Product",
                categoryId: "cat-1",
                categoryName: "Test Category",
                price: 29.99m,
                stockQuantity: 5
            );

            _mockProductRepository
                .Setup(r => r.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new[] { product }, 1));

            var query = new GetProductsQuery(Page: 1, PageSize: 10);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var dto = result.Products.First();
            dto.Id.Should().Be("test-id");
            dto.Name.Should().Be("Test Product");
            dto.CategoryId.Should().Be("cat-1");
            dto.CategoryName.Should().Be("Test Category");
            dto.Price.Should().Be(29.99m);
            dto.Currency.Should().Be("USD");
            dto.StockQuantity.Should().Be(5);
        }

        public void Dispose()
        {
            // Clean up resources if needed
        }
    }
}
