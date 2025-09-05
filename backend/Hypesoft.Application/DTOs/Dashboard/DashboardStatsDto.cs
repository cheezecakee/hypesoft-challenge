namespace Hypesoft.Application.DTOs.Dashboard
{
    public record DashboardStatsDto(
        int TotalProducts,
        decimal TotalStockValue,
        int LowStockProductCount,
        int TotalCategories
    );
}