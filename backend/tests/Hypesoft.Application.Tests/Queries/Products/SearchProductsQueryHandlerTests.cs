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
    public class SearchProductsQueryHandlerTests : IDisposable
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly IMapper _mapper;
        private readonly SearchProductsQueryHandler _handler;

        public SearchProductsQueryHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mapper = CreateMapper();
            _handler = new SearchProductsQueryHandler(_mockProductRepository.Object, _mapper);
        }

        private static IMapper CreateMapper()
        {
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
        public async Task Handle_WithSearchTermOnly_ShouldReturnPagedResults()
        {
            // Arrange
            var searchResults = new List<Product>
            {
                CreateTestProduct("1", "Gaming Laptop", "electronics", "Electronics", 1299.99m),
                CreateTestProduct("2", "Gaming Mouse", "accessories", "Accessories", 79.99m),
                CreateTestProduct("3", "Gaming Keyboard", "accessories", "Accessories", 129.99m)
            };

            _mockProductRepository
                .Setup(r => r.SearchAsync("Gaming", null, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((searchResults, 3));

            var query = new SearchProductsQuery("Gaming", null, 1, 10);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().HaveCount(3);
            result.TotalCount.Should().Be(3);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalPages.Should().Be(1); // 3 items, page size 10 = 1 page

            var productList = result.Products.ToList();
            productList.Should().OnlyContain(p => p.Name.Contains("Gaming"));
        }

        [Fact]
        public async Task Handle_WithCategoryIdOnly_ShouldFilterByCategory()
        {
            // Arrange
            var categoryProducts = new List<Product>
            {
                CreateTestProduct("1", "iPhone 15", "phones", "Phones", 999.99m),
                CreateTestProduct("2", "Samsung Galaxy", "phones", "Phones", 899.99m),
                CreateTestProduct("3", "Google Pixel", "phones", "Phones", 799.99m),
                CreateTestProduct("4", "OnePlus 12", "phones", "Phones", 699.99m)
            };

            _mockProductRepository
                .Setup(r => r.SearchAsync(null, "phones", 1, 3, It.IsAny<CancellationToken>()))
                .ReturnsAsync((categoryProducts.Take(3), 4)); // First page of 3 items

            var query = new SearchProductsQuery(null, "phones", 1, 3);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().HaveCount(3);
            result.TotalCount.Should().Be(4);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(3);
            result.TotalPages.Should().Be(2); // 4 items, page size 3 = 2 pages

            result.Products.Should().OnlyContain(p => p.CategoryId == "phones");
        }

        [Fact]
        public async Task Handle_WithBothSearchTermAndCategoryId_ShouldApplyBothFilters()
        {
            // Arrange
            var filteredResults = new List<Product>
            {
                CreateTestProduct("1", "Wireless Gaming Headset", "accessories", "Accessories", 199.99m),
                CreateTestProduct("2", "Gaming Chair Pro", "accessories", "Accessories", 399.99m)
            };

            _mockProductRepository
                .Setup(r => r.SearchAsync("Gaming", "accessories", 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((filteredResults, 2));

            var query = new SearchProductsQuery("Gaming", "accessories", 1, 10);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.TotalPages.Should().Be(1);

            result.Products.Should().OnlyContain(p => 
                p.Name.Contains("Gaming") && p.CategoryId == "accessories");
        }

        [Fact]
        public async Task Handle_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            var allResults = Enumerable.Range(1, 25)
                .Select(i => CreateTestProduct(i.ToString(), $"Product {i}", "cat1", "Category"))
                .ToList();

            var secondPageResults = allResults.Skip(10).Take(10); // Page 2, size 10
            
            _mockProductRepository
                .Setup(r => r.SearchAsync("Product", null, 2, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((secondPageResults, 25));

            var query = new SearchProductsQuery("Product", null, 2, 10);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().HaveCount(10);
            result.TotalCount.Should().Be(25);
            result.Page.Should().Be(2);
            result.PageSize.Should().Be(10);
            result.TotalPages.Should().Be(3); // 25 items, page size 10 = 3 pages

            var productList = result.Products.ToList();
            productList.First().Name.Should().Be("Product 11"); // First item on page 2
            productList.Last().Name.Should().Be("Product 20");  // Last item on page 2
        }

        [Fact]
        public async Task Handle_WithLastPageNotFull_ShouldCalculatePagesCorrectly()
        {
            // Arrange - 7 items with page size 3 = 3 pages (3, 3, 1)
            var lastPageResults = new List<Product>
            {
                CreateTestProduct("7", "Product 7", "cat1", "Category")
            };

            _mockProductRepository
                .Setup(r => r.SearchAsync("Product", null, 3, 3, It.IsAny<CancellationToken>()))
                .ReturnsAsync((lastPageResults, 7));

            var query = new SearchProductsQuery("Product", null, 3, 3);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().HaveCount(1); // Last page has only 1 item
            result.TotalCount.Should().Be(7);
            result.Page.Should().Be(3);
            result.PageSize.Should().Be(3);
            result.TotalPages.Should().Be(3); // Ceiling(7/3) = 3 pages
        }

        [Fact]
        public async Task Handle_WithNoResults_ShouldReturnEmptyPagedResult()
        {
            // Arrange
            _mockProductRepository
                .Setup(r => r.SearchAsync("NonExistentProduct", null, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Product>(), 0));

            var query = new SearchProductsQuery("NonExistentProduct", null, 1, 10);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalPages.Should().Be(0);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Handle_WithEmptySearchTerm_ShouldPassNullToRepository(string? searchTerm)
        {
            // Arrange
            var products = new List<Product>
            {
                CreateTestProduct("1", "Product 1", "cat1", "Category")
            };

            _mockProductRepository
                .Setup(r => r.SearchAsync(null, null, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((products, 1));

            var query = new SearchProductsQuery(searchTerm, null, 1, 10);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().HaveCount(1);

            // Verify the repository was called with null for empty/whitespace search terms
            _mockProductRepository.Verify(
                r => r.SearchAsync(null, null, 1, 10, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldMapProductsCorrectly()
        {
            // Arrange
            var product = CreateTestProduct(
                id: "test-id",
                name: "Premium Headphones",
                categoryId: "audio",
                categoryName: "Audio Equipment",
                price: 299.99m,
                stockQuantity: 15
            );

            _mockProductRepository
                .Setup(r => r.SearchAsync("Premium", null, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new[] { product }, 1));

            var query = new SearchProductsQuery("Premium", null, 1, 10);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var dto = result.Products.First();
            dto.Id.Should().Be("test-id");
            dto.Name.Should().Be("Premium Headphones");
            dto.CategoryId.Should().Be("audio");
            dto.CategoryName.Should().Be("Audio Equipment");
            dto.Price.Should().Be(299.99m);
            dto.Currency.Should().Be("USD");
            dto.StockQuantity.Should().Be(15);
        }

        [Theory]
        [InlineData(1, 10, 1)]   // 1 item, page size 10 = 1 page
        [InlineData(10, 10, 1)]  // 10 items, page size 10 = 1 page
        [InlineData(11, 10, 2)]  // 11 items, page size 10 = 2 pages
        [InlineData(25, 7, 4)]   // 25 items, page size 7 = 4 pages
        [InlineData(0, 10, 0)]   // 0 items = 0 pages
        public async Task Handle_ShouldCalculateTotalPagesCorrectly(int totalItems, int pageSize, int expectedPages)
        {
            // Arrange
            var products = Enumerable.Range(1, Math.Min(totalItems, pageSize))
                .Select(i => CreateTestProduct(i.ToString(), $"Product {i}", "cat1", "Category"))
                .ToList();

            _mockProductRepository
                .Setup(r => r.SearchAsync("test", null, 1, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync((products, totalItems));

            var query = new SearchProductsQuery("test", null, 1, pageSize);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.TotalPages.Should().Be(expectedPages);
            result.TotalCount.Should().Be(totalItems);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrows_ShouldPropagateException()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Search index unavailable");
            
            _mockProductRepository
                .Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            var query = new SearchProductsQuery("test", null, 1, 10);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
            
            exception.Message.Should().Be("Search index unavailable");
        }

        [Fact]
        public async Task Handle_WhenCancellationRequested_ShouldThrowOperationCanceledException()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockProductRepository
                .Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, int, int, CancellationToken>(async (searchTerm, categoryId, page, pageSize, ct) =>
                {
                    ct.ThrowIfCancellationRequested();
                    await Task.Delay(1, ct);
                    return (new List<Product>(), 0);
                });

            var query = new SearchProductsQuery("test", null, 1, 10);

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _handler.Handle(query, cts.Token));
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectParametersToRepository()
        {
            // Arrange
            const string searchTerm = "laptop";
            const string categoryId = "electronics";
            const int page = 2;
            const int pageSize = 5;

            _mockProductRepository
                .Setup(r => r.SearchAsync(searchTerm, categoryId, page, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Product>(), 0));

            var query = new SearchProductsQuery(searchTerm, categoryId, page, pageSize);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockProductRepository.Verify(
                r => r.SearchAsync(searchTerm, categoryId, page, pageSize, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        public void Dispose()
        {
            // Clean up resources if needed
        }
    }
}
