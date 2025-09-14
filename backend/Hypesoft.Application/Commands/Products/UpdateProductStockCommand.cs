using Hypesoft.Application.DTOs.Products;
using MediatR;

namespace Hypesoft.Application.Commands.Products
{
    public record UpdateProductStockCommand(
            string Id,
            int StockQuantity)
        : IRequest<ProductDto?>;
}
