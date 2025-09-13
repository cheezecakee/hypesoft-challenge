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
    public class UpdateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UpdateCategoryCommandHandler _handler;

        public UpdateCategoryCommandHandlerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new UpdateCategoryCommandHandler(
                _mockCategoryRepository.Object,
                _mockMapper.Object);
        }

     
        [Fact]
        public async Task Handle_FullUpdate_ShouldUpdateCategoryAndReturnDto()
        {
            // Arrange
            var categoryId = "category-123";
            var command = new UpdateCategoryCommand(categoryId, "New Name", "New Description");

            // Make sure the existing category has the same ID as the command
            var existingCategory = new Category("Old Name", "Old Description");
            typeof(Category).GetProperty("Id")!
                .SetValue(existingCategory, categoryId);

            // Expected DTO must include all required fields
            var expectedDto = new CategoryDto(
                categoryId,
                "New Name",
                "New Description",
                0,                // ProductCount
                existingCategory.CreateAt,
                existingCategory.UpdatedAt
            );

            _mockCategoryRepository
                .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            _mockCategoryRepository
                .Setup(x => x.GetByNameAsync("New Name", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            _mockCategoryRepository
                .Setup(x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            _mockMapper
                .Setup(x => x.Map<CategoryDto>(existingCategory))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDto);

            _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
            _mockCategoryRepository.Verify(x => x.GetByNameAsync("New Name", It.IsAny<CancellationToken>()), Times.Once);
            _mockCategoryRepository.Verify(x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<CategoryDto>(existingCategory), Times.Once);
        }

        [Fact]
        public async Task Handle_DuplicateName_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var existingCategory = new Category("Old Name", "Old Description");
            var conflictingCategory = new Category("Existing Name", "Other Description");

            var command = new UpdateCategoryCommand(existingCategory.Id, "Existing Name", "Description");

            _mockCategoryRepository
                .Setup(x => x.GetByIdAsync(existingCategory.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            _mockCategoryRepository
                .Setup(x => x.GetByNameAsync("Existing Name", It.IsAny<CancellationToken>()))
                .ReturnsAsync(conflictingCategory);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_PartialUpdate_ShouldUpdateOnlyProvidedFields()
        {
            // Arrange
            var category = new Category("Original Name", "Original Description");

            var command = new UpdateCategoryCommand(category.Id, null, "Updated Description");

            var expectedDto = new CategoryDto(
                category.Id,
                "Original Name",          // Name remains unchanged
                "Updated Description",    // Description updated
                category.Products.Count,
                category.CreateAt,
                category.UpdatedAt
            );

            _mockCategoryRepository
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _mockCategoryRepository
                .Setup(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            _mockCategoryRepository
                .Setup(x => x.UpdateAsync(category, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _mockMapper
                .Setup(x => x.Map<CategoryDto>(category))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedDto);
            category.Name.Should().Be("Original Name");           // unchanged
            category.Description.Should().Be("Updated Description"); // updated
        }
    }
}

