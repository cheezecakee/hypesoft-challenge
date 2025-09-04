namespace Hypesoft.Application.DTOs.Categories
{
    public record CategoryDto(
        string Id,
        string Name,
        string Description,
        int ProductCount,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}

