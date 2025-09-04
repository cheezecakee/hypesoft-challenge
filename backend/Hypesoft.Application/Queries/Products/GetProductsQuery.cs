using Hypesoft.Application.DTOs.Products;
using MediatR;

namespace Hypesoft.Application.Queries.Products
{
    public record GetProductsQuery(
        int Page = 1,
        int PageSize = 10,
        string? SearchName = null,
        string? CategoryId = null
    ) : IRequest<(IEnumerable<ProductDto> Products, int TotalCount)>;
}
