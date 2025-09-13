using Hypesoft.Application.DTOs.Dashboard;
using Hypesoft.Application.Handlers.Dashboard;
using Hypesoft.Application.Queries.Dashboard;
using Hypesoft.Domain.Services;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Hypesoft.Application.Tests.Queries.Dashboard
{
    public class GetProductsByCategoryQueryHandlerTests
    {
        private readonly Mock<IDashboardService> _mockDashboardService;
        private readonly GetProductsByCategoryQueryHandler _handler;

        public GetProductsByCategoryQueryHandlerTests()
        {
            _mockDashboardService = new Mock<IDashboardService>();
            _handler = new GetProductsByCategoryQueryHandler(_mockDashboardService.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCategoryStats()
        {
            // Arrange
            var categoryStatsData = new List<(string, string, int, decimal)>
            {
                ("cat1", "Electronics", 10, 10000m),
                ("cat2", "Accessories", 5, 500m)
            };

            _mockDashboardService
                .Setup(s => s.GetProductsByCategoryAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoryStatsData);

            var query = new GetProductsByCategoryQuery();

            // Act
            IEnumerable<CategoryStatsDto> result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var list = result.ToList();
            list[0].CategoryId.Should().Be("cat1");
            list[0].CategoryName.Should().Be("Electronics");
            list[0].ProductCount.Should().Be(10);
            list[0].TotalValue.Should().Be(10000m);

            list[1].CategoryId.Should().Be("cat2");
            list[1].CategoryName.Should().Be("Accessories");
            list[1].ProductCount.Should().Be(5);
            list[1].TotalValue.Should().Be(500m);
        }

        [Fact]
        public async Task Handle_WhenServiceThrows_ShouldPropagateException()
        {
            // Arrange
            var exception = new InvalidOperationException("Service error");
            _mockDashboardService
                .Setup(s => s.GetProductsByCategoryAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var query = new GetProductsByCategoryQuery();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            _mockDashboardService
                .Setup(s => s.GetProductsByCategoryAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<(string, string, int, decimal)>());

            var query = new GetProductsByCategoryQuery();

            // Act
            IEnumerable<CategoryStatsDto> result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
