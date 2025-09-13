using Xunit;
using Moq;
using MediatR;
using Hypesoft.API.Controllers;
using Hypesoft.Application.Queries.Categories;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Application.DTOs.Categories;
using Microsoft.AspNetCore.Mvc;

namespace Hypesoft.API.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CategoriesController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            var categories = new[]
            {
                new CategoryDto("1", "Cat1", "Desc1", 5, DateTime.UtcNow, null)
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoriesQuery>(), default))
                .ReturnsAsync(categories);

            var result = await _controller.GetCategories();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategories = Assert.IsAssignableFrom<IEnumerable<CategoryDto>>(okResult.Value);
            Assert.Single(returnedCategories);
        }

        [Fact]
        public async Task GetCategory_ReturnsCategory_WhenFound()
        {
            var category = new CategoryDto("1", "Cat1", "Desc1", 5, DateTime.UtcNow, null);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), default))
                .ReturnsAsync(category);

            var result = await _controller.GetCategory("1");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategory = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal("1", returnedCategory.Id);
        }

        [Fact]
        public async Task GetCategory_ReturnsNotFound_WhenMissing()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), default))
                .ReturnsAsync((CategoryDto?)null);

            var result = await _controller.GetCategory("missing");

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("missing", notFound.Value.ToString());
        }

        [Fact]
        public async Task CreateCategory_ReturnsCreatedCategory()
        {
            var categoryDto = new CategoryDto("1", "Cat1", "Desc1", 0, DateTime.UtcNow, null);
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCategoryCommand>(), default))
                .ReturnsAsync(categoryDto);

            var command = new CreateCategoryCommand("Cat1", "Desc1");

            var result = await _controller.CreateCategory(command);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedCategory = Assert.IsType<CategoryDto>(createdAtResult.Value);
            Assert.Equal("1", returnedCategory.Id);
        }

        [Fact]
        public async Task UpdateCategory_ReturnsUpdatedCategory()
        {
            var updated = new CategoryDto("1", "Updated", "DescUpdated", 0, DateTime.UtcNow, null);
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCategoryCommand>(), default))
                .ReturnsAsync(updated);

            var dto = new UpdateCategoryDto("Updated", "DescUpdated");

            var result = await _controller.UpdateCategory("1", dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal("Updated", returned.Name);
        }

        [Fact]
        public async Task PatchCategory_ReturnsPatchedCategory()
        {
            var patched = new CategoryDto("1", "Patched", "DescPatched", 0, DateTime.UtcNow, null);
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCategoryCommand>(), default))
                .ReturnsAsync(patched);

            var dto = new UpdateCategoryDto("Patched", "DescPatched");

            var result = await _controller.PatchCategory("1", dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal("Patched", returned.Name);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNoContent()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCategoryCommand>(), default))
                .ReturnsAsync(true);

            var result = await _controller.DeleteCategory("1");

            Assert.IsType<NoContentResult>(result);
        }
    }
}
