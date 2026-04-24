using JobPortal.API.Models;
using JobPortal.Application.Features.Dashboard.Queries.GetApplicationTrend;
using JobPortal.Application.Features.Dashboard.Queries.GetStats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken ct)
    {
        var result = await mediator.Send(new GetDashboardStatsQuery(), ct);
        return Ok(ApiResponse<DashboardStatsResponse>.Ok(result, "Dashboard stats retrieved."));
    }

    /// <summary>Get application submission counts per day for the last N days.</summary>
    [HttpGet("application-trend")]
    public async Task<IActionResult> GetApplicationTrend(
        [FromQuery] int days = 30,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetApplicationTrendQuery(days), ct);
        return Ok(ApiResponse<List<ApplicationTrendPoint>>.Ok(result, "Trend retrieved."));
    }
}
