using AutoMapper;
using FluentAssertions;
using Hypesoft.Application.Common.Mappings;
using Hypesoft.Application.DTOs.Categories;
using Hypesoft.Application.Handlers.Categories;
using Hypesoft.Application.Queries.Categories;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Common; 
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Hypesoft.Application.Tests.Queries.Categories
{
    public class GetCategoriesQueryHandlerTests : IDisposable
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly IMapper _mapper;
        private readonly GetCategoriesQueryHandler _handler;

        public GetCategoriesQueryHandlerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mapper = CreateMapper();
            _handler = new GetCategoriesQueryHandler(_mockCategoryRepository.Object, _mapper);
        }

        private static IMapper CreateMapper()
        {
            using var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { });
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), loggerFactory);
            return config.CreateMapper();
        }

        private static Category CreateTestCategory(string id, string name, string description, int productCount = 0)
        {
            var category = new Category(name, description);

            // Set the Id property via reflection since it's private in AggregateRoot
            var idProperty = typeof(AggregateRoot).GetProperty("Id");
            if (idProperty != null && idProperty.CanWrite)
            {
                idProperty.SetValue(category, id);
            }

            // Optionally populate Products to match ProductCount
            for (int i = 0; i < productCount; i++)
            {
                // Products can be empty objects just to fill the count
                category.Products.Add(new Product("Dummy", "Desc", new Hypesoft.Domain.ValueObjects.Money(1, "USD"), "dummy", 1));
            }

            return category;
        }

        [Fact]
        public async Task Handle_ShouldReturnAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                CreateTestCategory("1", "Electronics", "Devices", 3),
                CreateTestCategory("2", "Furniture", "Home items", 5)
            };

            var productCounts = new Dictionary<string, int>
            {
                {"1", 3},
                {"2", 5}
            };

            _mockCategoryRepository
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            _mockCategoryRepository
                .Setup(r => r.GetAllWithProductCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(productCounts);

            var query = new GetCategoriesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            var list = result.ToList();
            list[0].Name.Should().Be("Electronics");
            list[0].ProductCount.Should().Be(3);
            list[1].Name.Should().Be("Furniture");
            list[1].ProductCount.Should().Be(5);

            _mockCategoryRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockCategoryRepository.Verify(r => r.GetAllWithProductCountAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNoCategories_ShouldReturnEmptyList()
        {
            // Arrange
            _mockCategoryRepository
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Category>());

            _mockCategoryRepository
                .Setup(r => r.GetAllWithProductCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, int>());

            var query = new GetCategoriesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrows_ShouldPropagateException()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Database unavailable");
            _mockCategoryRepository
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            var query = new GetCategoriesQuery();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
            
            ex.Message.Should().Be("Database unavailable");
        }

        public void Dispose()
        {
            // Cleanup if necessary
        }
    }
}
