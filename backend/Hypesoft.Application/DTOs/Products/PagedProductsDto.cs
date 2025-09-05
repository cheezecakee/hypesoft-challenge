namespace Hypesoft.Application.DTOs.Products
{
    public record PagedProductsDto(
        IEnumerable<ProductDto> Products,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    )
    {
        public PagedProductsDto() : this(Enumerable.Empty<ProductDto>(), 0, 1, 10, 0) { }
    };
}
