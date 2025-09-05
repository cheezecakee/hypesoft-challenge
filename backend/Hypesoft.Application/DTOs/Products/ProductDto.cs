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
    )
    {
        public ProductDto() : this(string.Empty, string.Empty, string.Empty, 0m, string.Empty,
            string.Empty, string.Empty, 0, false, DateTime.MinValue, null)
        { }
    };
}
