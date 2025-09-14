using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.Queries.Products;
using Hypesoft.Application.DTOs.Products;

namespace Hypesoft.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AuthenticatedUser")]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<PagedProductsDto>> GetProducts(
            [FromQuery] string? search,
            [FromQuery] string? categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            SearchProductsQuery query = new(search, categoryId, page, pageSize);
            PagedProductsDto result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(string id)
        {
            GetProductByIdQuery query = new(id);
            ProductDto? result = await _mediator.Send(query);
            return result == null
                ? NotFound($"Product with ID {id} not found")
                : Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductCommand command)
        {
            ProductDto result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(string id, [FromBody] UpdateProductDto dto)
        {
            UpdateProductCommand command = new(
                id,
                dto.Name,
                dto.Description,
                dto.Price,
                dto.Currency,
                dto.CategoryId,
                dto.StockQuantity
                );
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<ActionResult<ProductDto>> PatchProduct(string id, [FromBody] UpdateProductDto dto)
        {
            UpdateProductCommand command = new(
                id,
                dto.Name,
                dto.Description,
                dto.Price,
                dto.Currency,
                dto.CategoryId,
                dto.StockQuantity
                );
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPatch("{id}/stock")]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<ActionResult<ProductDto>> UpdateProductStock(string id, [FromBody] UpdateProductStockDto dto)
        {
            UpdateProductStockCommand command = new(id, dto.StockQuantity);
            ProductDto? result = await _mediator.Send(command);
            return result == null
                ? NotFound()
                : Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            DeleteProductCommand command = new(id);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStockProducts()
        {
            GetLowStockProductsQuery query = new();
            IEnumerable<ProductDto> result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
