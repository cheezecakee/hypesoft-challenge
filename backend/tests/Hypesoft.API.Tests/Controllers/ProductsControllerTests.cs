using Xunit;
using Moq;
using MediatR;
using Hypesoft.API.Controllers;
using Hypesoft.Application.Queries.Products;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.DTOs.Products;
using Microsoft.AspNetCore.Mvc;

namespace Hypesoft.API.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ProductsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsPagedProductsDto()
        {
            // Arrange
            var pagedDto = new PagedProductsDto(
                new[] { new ProductDto("1", "Prod1", "Desc", 10, "USD", "cat1", "Category 1", 5, false, DateTime.UtcNow, null) },
                1, 1, 10, 1
            );
            _mediatorMock.Setup(m => m.Send(It.IsAny<SearchProductsQuery>(), default))
                .ReturnsAsync(pagedDto);

            // Act
            var result = await _controller.GetProducts(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<PagedProductsDto>(okResult.Value);
            Assert.Single(returnedDto.Products); // <-- updated to Products
        }

        [Fact]
        public async Task GetProduct_ReturnsProduct_WhenFound()
        {
            var product = new ProductDto("1", "Prod1", "Desc", 10, "USD", "cat1", "Category 1", 5, false, DateTime.UtcNow, null);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), default))
                .ReturnsAsync(product);

            var result = await _controller.GetProduct("1");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal("1", returnedProduct.Id);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenMissing()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), default))
                .ReturnsAsync((ProductDto?)null);

            var result = await _controller.GetProduct("missing");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("missing", notFoundResult.Value!.ToString());
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedProduct()
        {
            var dto = new ProductDto("1", "Prod1", "Desc", 10, "USD", "cat1", "Category 1", 5, false, DateTime.UtcNow, null);
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ReturnsAsync(dto);

            var command = new CreateProductCommand("Prod1", "Desc", 10, "USD", "cat1", 5);

            var result = await _controller.CreateProduct(command);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(createdAtResult.Value);
            Assert.Equal("1", returnedProduct.Id);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsUpdatedProduct()
        {
            var dto = new ProductDto("1", "Updated", "Desc", 20, "USD", "cat1", "Category 1", 5, false, DateTime.UtcNow, null);
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ReturnsAsync(dto);

            var updateDto = new UpdateProductDto("Updated", "Desc", 20, "USD", "cat1", 5);

            var result = await _controller.UpdateProduct("1", updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal("Updated", returnedProduct.Name);
        }

        [Fact]
        public async Task PatchProduct_ReturnsPatchedProduct()
        {
            var dto = new ProductDto("1", "Patched", "Desc", 20, "USD", "cat1", "Category 1", 5, false, DateTime.UtcNow, null);
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ReturnsAsync(dto);

            var patchDto = new UpdateProductDto("Patched", "Desc", 20, "USD", "cat1", 5);

            var result = await _controller.PatchProduct("1", patchDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal("Patched", returnedProduct.Name);
        }

        [Fact]
        public async Task UpdateProductStock_ReturnsOkResult()
        {
            var stockDto = new UpdateProductStockDto(10);
            var expectedDto = new ProductDto("1", "Product", "Desc", 10, "USD", "cat1", "Category 1", 10, false, DateTime.UtcNow, null);
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProductStockCommand>(), default))
                .ReturnsAsync(expectedDto);

            var result = await _controller.UpdateProductStock("1", stockDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal("1", returnedProduct.Id);
        }

        [Fact]
        public async Task UpdateProductStock_ReturnsNotFound_WhenProductNotExists()
        {
            var stockDto = new UpdateProductStockDto(10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProductStockCommand>(), default))
                .ReturnsAsync((ProductDto?)null);

            var result = await _controller.UpdateProductStock("nonexistent", stockDto);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ReturnsAsync(true);

            var result = await _controller.DeleteProduct("1");

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetLowStockProducts_ReturnsProducts()
        {
            var products = new[]
            {
                new ProductDto("1", "Prod1", "Desc", 10, "USD", "cat1", "Category 1", 5, true, DateTime.UtcNow, null)
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetLowStockProductsQuery>(), default))
                .ReturnsAsync(products);

            var result = await _controller.GetLowStockProducts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Single(returnedProducts);
        }
    }
}
