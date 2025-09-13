using Hypesoft.Domain.Entities;

namespace Hypesoft.Domain.Tests.Entities
{
    public class CategoryTests
    {
        [Fact]
        public void Constructor_ValidValues_ShouldCreateCategory()
        {
            // Arrange
            var name = "Electronics";
            var description = "Electronic devices and accessories";

            // Act
            var category = new Category(name, description);

            // Assert
            category.Name.Should().Be(name);
            category.Description.Should().Be(description);
            category.Id.Should().NotBeNullOrEmpty();
            category.CreateAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_EmptyOrNullName_ShouldThrowException(string invalidName)
        {
            // Arrange
            var description = "Valid description";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Category(invalidName, description));
            exception.Message.Should().Contain("Category name is required");
        }

        [Fact]
        public void Constructor_NameTooLong_ShouldThrowException()
        {
            // Arrange
            var longName = new string('a', 101); // 101 characters
            var description = "Valid description";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Category(longName, description));
            exception.Message.Should().Contain("Category name cannot exceed 100 characters");
        }

        [Fact]
        public void Constructor_NullDescription_ShouldSetEmptyDescription()
        {
            // Arrange
            var name = "Electronics";
            string nullDescription = null!;

            // Act
            var category = new Category(name, nullDescription);

            // Assert
            category.Description.Should().Be(string.Empty);
        }

        [Fact]
        public void Constructor_NameWithWhitespace_ShouldTrimName()
        {
            // Arrange
            var nameWithWhitespace = "  Electronics  ";
            var description = "Description";

            // Act
            var category = new Category(nameWithWhitespace, description);

            // Assert
            category.Name.Should().Be("Electronics");
        }

        [Fact]
        public void Update_ValidValues_ShouldUpdateCategory()
        {
            // Arrange
            var category = new Category("Old Name", "Old Description");
            var originalUpdatedAt = category.UpdatedAt;
            var newName = "New Name";
            var newDescription = "New Description";

            // Act
            category.Update(newName, newDescription);

            // Assert
            category.Name.Should().Be(newName);
            category.Description.Should().Be(newDescription);
            category.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }

        [Fact]
        public void PartialUpdate_OnlyName_ShouldUpdateOnlyName()
        {
            // Arrange
            var category = new Category("Old Name", "Old Description");
            var originalDescription = category.Description;
            var originalUpdatedAt = category.UpdatedAt;

            // Act
            category.PartialUpdate(name: "New Name");

            // Assert
            category.Name.Should().Be("New Name");
            category.Description.Should().Be(originalDescription);
            category.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }

        [Fact]
        public void PartialUpdate_OnlyDescription_ShouldUpdateOnlyDescription()
        {
            // Arrange
            var category = new Category("Old Name", "Old Description");
            var originalName = category.Name;
            var originalUpdatedAt = category.UpdatedAt;

            // Act
            category.PartialUpdate(description: "New Description");

            // Assert
            category.Name.Should().Be(originalName);
            category.Description.Should().Be("New Description");
            category.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }

        [Fact]
        public void PartialUpdate_NoChanges_ShouldNotUpdateTimestamp()
        {
            // Arrange
            var category = new Category("Test Category", "Test Description");
            var originalUpdatedAt = category.UpdatedAt;

            // Act
            category.PartialUpdate(name: "Test Category", description: "Test Description");

            // Assert
            category.UpdatedAt.Should().Be(originalUpdatedAt);
        }

        [Fact]
        public void PartialUpdate_BothParameters_ShouldUpdateBothFields()
        {
            // Arrange
            var category = new Category("Old Name", "Old Description");
            var originalUpdatedAt = category.UpdatedAt;

            // Act
            category.PartialUpdate(name: "New Name", description: "New Description");

            // Assert
            category.Name.Should().Be("New Name");
            category.Description.Should().Be("New Description");
            category.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }
    }
}
