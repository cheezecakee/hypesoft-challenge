
using FluentAssertions;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.Handlers.Products;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Hypesoft.Application.Tests.Commands.Products
{
    public class DeleteProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _handler = new DeleteProductCommandHandler(_mockProductRepository.Object);
        }

        [Fact]
        public async Task Handle_ProductExists_ShouldDeleteAndReturnTrue()
        {
            // Arrange
            var productId = "product-123";

            _mockProductRepository
                .Setup(x => x.ExistsAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockProductRepository
                .Setup(x => x.DeleteAsync(productId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var command = new DeleteProductCommand(productId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _mockProductRepository.Verify(x => x.ExistsAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            _mockProductRepository.Verify(x => x.DeleteAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ProductDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var productId = "nonexistent-product";

            _mockProductRepository
                .Setup(x => x.ExistsAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var command = new DeleteProductCommand(productId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _mockProductRepository.Verify(x => x.ExistsAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            _mockProductRepository.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var productId = "product-123";

            _mockProductRepository
                .Setup(x => x.ExistsAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockProductRepository
                .Setup(x => x.DeleteAsync(productId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new System.InvalidOperationException("Database error"));

            var command = new DeleteProductCommand(productId);

            // Act & Assert
            await Assert.ThrowsAsync<System.InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CancellationRequested_ShouldRespectCancellation()
        {
            // Arrange
            var productId = "product-123";
            var cancellationToken = new CancellationToken(true);

            _mockProductRepository
                .Setup(x => x.ExistsAsync(productId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new System.OperationCanceledException());

            var command = new DeleteProductCommand(productId);

            // Act & Assert
            await Assert.ThrowsAsync<System.OperationCanceledException>(() =>
                _handler.Handle(command, cancellationToken));
        }
    }
}

