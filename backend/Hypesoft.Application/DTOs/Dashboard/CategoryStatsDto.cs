namespace Hypesoft.Application.DTOs.Dashboard
{
    public record CategoryStatsDto(
        string CategoryId,
        string CategoryName,
        int ProductCount,
        decimal TotalValue
    );
}