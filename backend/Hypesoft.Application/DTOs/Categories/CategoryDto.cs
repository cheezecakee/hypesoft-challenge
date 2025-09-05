namespace Hypesoft.Application.DTOs.Categories
{
    public record CategoryDto(
        string Id,
        string Name,
        string Description,
        int ProductCount,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    )
    {
        public CategoryDto() : this(string.Empty, string.Empty, string.Empty, 0, DateTime.MinValue, null) { }
    };
}
