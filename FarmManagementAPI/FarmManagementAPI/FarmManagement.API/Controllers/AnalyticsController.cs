using FarmManagement.Core.Interfaces;
using FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;
using FarmManagementAPI.FarmManagement.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly IFarmRepository _farmRepository;

    public AnalyticsController(IAnalyticsService analyticsService, IFarmRepository farmRepository)
    {
        _analyticsService = analyticsService;
        _farmRepository = farmRepository;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<FarmSummaryDto>> GetFarmSummary()
    {
        var userId = GetCurrentUserId();
        var summary = await _analyticsService.GetFarmSummaryAsync(userId);
        return Ok(summary);
    }

    [HttpGet("yield-trends")]
    public async Task<ActionResult<IEnumerable<YieldTrendDto>>> GetYieldTrends([FromQuery] int months = 6)
    {
        var userId = GetCurrentUserId();
        var trends = await _analyticsService.GetYieldTrendsAsync(userId, months);
        return Ok(trends);
    }

    [HttpGet("efficiency-metrics")]
    public async Task<ActionResult<EfficiencyMetricsDto>> GetEfficiencyMetrics()
    {
        var userId = GetCurrentUserId();
        var metrics = await _analyticsService.GetEfficiencyMetricsAsync(userId);
        return Ok(metrics);
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userId!);
    }
}