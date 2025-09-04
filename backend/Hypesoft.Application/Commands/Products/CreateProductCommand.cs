using Hypesoft.Application.DTOs.Products;
using MediatR;

namespace Hypesoft.Application.Commands.Products
{
    public record CreateProductCommand(
        string Name,
        string Description,
        decimal Price,
        string Currency,
        string CategoryId,
        int StockQuantity
    ) : IRequest<ProductDto>;
}
