using Hypesoft.Application.DTOs.Products;
using MediatR;

namespace Hypesoft.Application.Commands.Products
{
    public record UpdateProductCommand(
        string Id,
        string? Name = null,
        string? Description = null,
        decimal? Price = null,
        string? Currency = null,
        string? CategoryId = null,
        int? StockQuantity = null
    ) : IRequest<ProductDto>;
}
