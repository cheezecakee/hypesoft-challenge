using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Hypesoft.Application.Queries.Dashboard;
using Hypesoft.Application.DTOs.Dashboard;

namespace Hypesoft.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AuthenticatedUser")]
    public class DashboardController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var query = new GetDashboardStatsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("products-by-category")]
        public async Task<ActionResult<IEnumerable<CategoryStatsDto>>> GetProductsByCategory()
        {
            var query = new GetProductsByCategoryQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
