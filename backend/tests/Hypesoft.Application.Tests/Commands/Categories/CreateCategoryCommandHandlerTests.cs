using AutoMapper;
using FluentAssertions;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Application.DTOs.Categories;
using Hypesoft.Application.Handlers.Categories;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Moq;
using Xunit;

namespace Hypesoft.Application.Tests.Commands.Categories
{
    public class CreateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CreateCategoryCommandHandler _handler;

        public CreateCategoryCommandHandlerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new CreateCategoryCommandHandler(
                _mockCategoryRepository.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateCategoryAndReturnDto()
        {
            // Arrange
            var command = new CreateCategoryCommand("Electronics", "All electronic products");

            var createdCategory = new Category(command.Name, command.Description);
            var expectedDto = new CategoryDto
            {
                Id = "cat-123",
                Name = command.Name,
                Description = command.Description
            };

            _mockCategoryRepository
                .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            _mockCategoryRepository
                .Setup(x => x.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdCategory);

            _mockMapper
                .Setup(x => x.Map<CategoryDto>(createdCategory))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedDto);
            _mockCategoryRepository.Verify(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()), Times.Once);
            _mockCategoryRepository.Verify(x => x.CreateAsync(It.Is<Category>(c => c.Name == command.Name), It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<CategoryDto>(createdCategory), Times.Once);
        }

        [Fact]
        public async Task Handle_CategoryNameAlreadyExists_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new CreateCategoryCommand("Electronics", "Description");

            var existingCategory = new Category(command.Name, command.Description);

            _mockCategoryRepository
                .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain($"Category with name '{command.Name}' already exists");
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new CreateCategoryCommand("Electronics", "Description");

            _mockCategoryRepository
                .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            _mockCategoryRepository
                .Setup(x => x.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
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
            var command = new CreateCategoryCommand("Electronics", "Description");
            var cancellationToken = new CancellationToken(true);

            _mockCategoryRepository
                .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => _handler.Handle(command, cancellationToken));
        }
    }
}
