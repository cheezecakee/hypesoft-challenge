using Hypesoft.Application.DTOs.Products;
using MediatR;

namespace Hypesoft.Application.Queries.Products
{
    public record GetLowStockProductsQuery() : IRequest<IEnumerable<ProductDto>>;
}
