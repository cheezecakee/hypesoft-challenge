namespace Hypesoft.Application.DTOs.Products
{
    public record ProductDto(
        string Id,
        string Name,
        string Description,
        decimal Price,
        string Currency,
        string CategoryId,
        string CategoryName,
        int StockQuantity,
        bool IsLowStock,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
