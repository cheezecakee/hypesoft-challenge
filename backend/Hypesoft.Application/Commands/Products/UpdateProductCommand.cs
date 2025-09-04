using Hypesoft.Application.DTOs.Products;
using MediatR;

namespace Hypesoft.Application.Commands.Products
{
    public record UpdateProductCommand(
        string Id,
        string Name,
        string Description,
        decimal Price,
        string Currency,
        string CategoryId
    ) : IRequest<ProductDto>;
}
