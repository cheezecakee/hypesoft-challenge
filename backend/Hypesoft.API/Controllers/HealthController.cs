using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hypesoft.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController(HealthCheckService healthCheckService) : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService = healthCheckService;

        [HttpGet]
        public async Task<ActionResult> CheckHealth()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            var response = new
            {
                Status = report.Status.ToString(),
                Checks = report.Entries.Select(x => new
                {
                    Name = x.Key,
                    Status = x.Value.Status.ToString(),
                    Description = x.Value.Description,
                    Duration = x.Value.Duration.TotalMilliseconds
                }),
                TotalDuration = report.TotalDuration.TotalMilliseconds
            };

            return report.Status == HealthStatus.Healthy
                ? Ok(response)
                : StatusCode(503, response);
        }
    }
}
