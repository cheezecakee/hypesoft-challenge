using Hypesoft.Application.DTOs.Dashboard;
using Hypesoft.Application.Queries.Dashboard;
using Hypesoft.Domain.Services;
using MediatR;

namespace Hypesoft.Application.Handlers.Dashboard
{
    public class GetDashboardStatsQueryHandler(IDashboardService dashboardService)
        : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly IDashboardService _dashboardService = dashboardService;

        public async Task<DashboardStatsDto> Handle(
            GetDashboardStatsQuery request,
            CancellationToken cancellationToken)
        {
            int totalProducts = await _dashboardService.GetTotalProductsAsync(cancellationToken);
            decimal totalStockValue = await _dashboardService.GetTotalStockValueAsync(cancellationToken);
            int lowStockCount = await _dashboardService.GetLowStockProductCountAsync(cancellationToken);
            int totalCategories = await _dashboardService.GetTotalCategoriesAsync(cancellationToken);

            return new DashboardStatsDto(
                totalProducts,
                totalStockValue,
                lowStockCount,
                totalCategories);
        }
    }
}

