using Hypesoft.Application.DTOs.Dashboard;
using Hypesoft.Application.Queries.Dashboard;
using Hypesoft.Domain.Services;
using MediatR;

namespace Hypesoft.Application.Handlers.Dashboard
{
    public class GetProductsByCategoryQueryHandler(IDashboardService dashboardService)
        : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<CategoryStatsDto>>
    {
        private readonly IDashboardService _dashboardService = dashboardService;

        public async Task<IEnumerable<CategoryStatsDto>> Handle(
            GetProductsByCategoryQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<(string CategoryId, string CategoryName, int ProductCount, decimal TotalValue)>
                categoryStats = await _dashboardService.GetProductsByCategoryAsync(cancellationToken);

            return categoryStats.Select(static cs => new CategoryStatsDto(
                cs.CategoryId,
                cs.CategoryName,
                cs.ProductCount,
                cs.TotalValue));
        }
    }
}

