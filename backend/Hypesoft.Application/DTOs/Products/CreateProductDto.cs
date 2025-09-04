namespace Hypesoft.Application.DTOs.Products
{
    public record CreateProductDto(
        string Name,
        string Description,
        decimal Price,
        string Currency,
        string CategoryId,
        int StockQuantity
    );
}

