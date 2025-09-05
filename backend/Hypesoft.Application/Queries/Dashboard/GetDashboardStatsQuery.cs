using MediatR;
using Hypesoft.Application.DTOs.Dashboard;

namespace Hypesoft.Application.Queries.Dashboard
{
    public record GetDashboardStatsQuery() : IRequest<DashboardStatsDto>;
}