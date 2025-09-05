using MediatR;
using Hypesoft.Application.DTOs.Products;

namespace Hypesoft.Application.Queries.Products
{
    public record SearchProductsQuery(
        string? SearchTerm,
        string? CategoryId,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<PagedProductsDto>;
}