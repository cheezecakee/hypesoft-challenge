using AutoMapper;
using FluentAssertions;
using Hypesoft.Application.DTOs.Categories;
using Hypesoft.Application.Handlers.Categories;
using Hypesoft.Application.Queries.Categories;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Hypesoft.Application.Tests.Queries.Categories
{
    public class GetCategoryByIdQueryHandlerTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly IMapper _mapper;
        private readonly GetCategoryByIdQueryHandler _handler;

        public GetCategoryByIdQueryHandlerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mapper = CreateMapper();
            _handler = new GetCategoryByIdQueryHandler(_mockCategoryRepository.Object, _mapper);
        }

        private static IMapper CreateMapper()
        {
            using var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { });
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryDto>();
            }, loggerFactory);
            return config.CreateMapper();
        }

        [Fact]
        public async Task Handle_CategoryExists_ShouldReturnCategoryDto()
        {
            // Arrange
            var category = new Category("Electronics", "Electronic devices");
            var query = new GetCategoryByIdQuery(category.Id);

            _mockCategoryRepository
                .Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(category.Id);
            result.Name.Should().Be(category.Name);
            result.Description.Should().Be(category.Description);
        }

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var query = new GetCategoryByIdQuery("non-existent-id");

            _mockCategoryRepository
                .Setup(r => r.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
