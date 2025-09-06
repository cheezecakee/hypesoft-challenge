namespace Hypesoft.Application.DTOs.Products
{
    public record UpdateProductDto(
        string? Name = null,
        string? Description = null,
        decimal? Price = null,
        string? Currency = null,
        string? CategoryId = null,
        int? StockQuantity = null
    );
}
