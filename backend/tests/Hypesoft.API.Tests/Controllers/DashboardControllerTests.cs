using Xunit;
using Moq;
using MediatR;
using Hypesoft.API.Controllers;
using Hypesoft.Application.Queries.Dashboard;
using Hypesoft.Application.DTOs.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Hypesoft.API.Tests.Controllers
{
    public class DashboardControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new DashboardController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetDashboardStats_ReturnsStats()
        {
            var stats = new DashboardStatsDto(100, 5000m, 10, 5);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDashboardStatsQuery>(), default))
                .ReturnsAsync(stats);

            var result = await _controller.GetDashboardStats();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedStats = Assert.IsType<DashboardStatsDto>(okResult.Value);
            Assert.Equal(100, returnedStats.TotalProducts);
            Assert.Equal(5000m, returnedStats.TotalStockValue);
        }

        [Fact]
        public async Task GetProductsByCategory_ReturnsCategoryStats()
        {
            var categories = new[]
            {
                new CategoryStatsDto("1", "Cat1", 5, 1000m),
                new CategoryStatsDto("2", "Cat2", 10, 2500m)
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductsByCategoryQuery>(), default))
                .ReturnsAsync(categories);

            var result = await _controller.GetProductsByCategory();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategories = Assert.IsAssignableFrom<IEnumerable<CategoryStatsDto>>(okResult.Value);
            Assert.Collection(returnedCategories,
                c => Assert.Equal("Cat1", c.CategoryName),
                c => Assert.Equal("Cat2", c.CategoryName));
        }
    }
}
