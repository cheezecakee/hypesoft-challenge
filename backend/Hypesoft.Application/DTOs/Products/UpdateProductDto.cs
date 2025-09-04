namespace Hypesoft.Application.DTOs.Products
{
    public record UpdateProductDto(
        string Id,
        string Name,
        string Description,
        decimal Price,
        string Currency,
        string CategoryId
    );
}
