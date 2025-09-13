using Hypesoft.Application.DTOs.Dashboard;
using Hypesoft.Application.Handlers.Dashboard;
using Hypesoft.Application.Queries.Dashboard;
using Hypesoft.Domain.Services;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Hypesoft.Application.Tests.Queries.Dashboard
{
    public class GetDashboardStatsQueryHandlerTests
    {
        private readonly Mock<IDashboardService> _mockDashboardService;
        private readonly GetDashboardStatsQueryHandler _handler;

        public GetDashboardStatsQueryHandlerTests()
        {
            _mockDashboardService = new Mock<IDashboardService>();
            _handler = new GetDashboardStatsQueryHandler(_mockDashboardService.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectDashboardStats()
        {
            // Arrange
            _mockDashboardService.Setup(s => s.GetTotalProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(100);
            _mockDashboardService.Setup(s => s.GetTotalStockValueAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(50000m);
            _mockDashboardService.Setup(s => s.GetLowStockProductCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);
            _mockDashboardService.Setup(s => s.GetTotalCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(10);

            var query = new GetDashboardStatsQuery();

            // Act
            DashboardStatsDto result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TotalProducts.Should().Be(100);
            result.TotalStockValue.Should().Be(50000m);
            result.LowStockProductCount.Should().Be(5);
            result.TotalCategories.Should().Be(10);
        }

        [Fact]
        public async Task Handle_WhenServiceThrows_ShouldPropagateException()
        {
            // Arrange
            var exception = new InvalidOperationException("Service failure");
            _mockDashboardService.Setup(s => s.GetTotalProductsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var query = new GetDashboardStatsQuery();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
