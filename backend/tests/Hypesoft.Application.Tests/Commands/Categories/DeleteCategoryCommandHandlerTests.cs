using Hypesoft.Application.Commands.Categories;
using Hypesoft.Application.Handlers.Categories;
using Hypesoft.Domain.Repositories;
using Moq;
using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace Hypesoft.Application.Tests.Commands.Categories
{
    public class DeleteCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly DeleteCategoryCommandHandler _handler;

        public DeleteCategoryCommandHandlerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _handler = new DeleteCategoryCommandHandler(_mockCategoryRepository.Object);
        }

        [Fact]
        public async Task Handle_CategoryExistsAndNoProducts_ShouldReturnTrue()
        {
            // Arrange
            var categoryId = "category-123";
            var command = new DeleteCategoryCommand(categoryId);

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockCategoryRepository
                .Setup(x => x.HasProductsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _mockCategoryRepository.Verify(x => x.DeleteAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var categoryId = "nonexistent-category";
            var command = new DeleteCategoryCommand(categoryId);

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _mockCategoryRepository.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CategoryHasProducts_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var categoryId = "category-123";
            var command = new DeleteCategoryCommand(categoryId);

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockCategoryRepository
                .Setup(x => x.HasProductsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            _mockCategoryRepository.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task Handle_CancellationRequested_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var categoryId = "category-123";
            var command = new DeleteCategoryCommand(categoryId);
            var cancellationToken = new CancellationToken(true); // already cancelled

            _mockCategoryRepository
                .Setup(x => x.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => _handler.Handle(command, cancellationToken));

            _mockCategoryRepository.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
