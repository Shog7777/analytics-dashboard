using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsDashboard.Api.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize(Roles = Roles.AnyRole)]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>Total views, average time on page and bounce rate for the selected period.</summary>
    [HttpGet("kpis")]
    public async Task<IActionResult> GetKpis([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        return Ok(await _analyticsService.GetKpisAsync(from, to));
    }

    /// <summary>Daily views time series for the selected period.</summary>
    [HttpGet("daily-views")]
    public async Task<IActionResult> GetDailyViews([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        return Ok(await _analyticsService.GetDailyViewsAsync(from, to));
    }

    /// <summary>Top articles ranked by views.</summary>
    [HttpGet("top-articles")]
    public async Task<IActionResult> GetTopArticles([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int limit = 10)
    {
        return Ok(await _analyticsService.GetTopArticlesAsync(from, to, limit));
    }

    /// <summary>Top tags ranked by aggregated views across tagged articles.</summary>
    [HttpGet("top-tags")]
    public async Task<IActionResult> GetTopTags([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int limit = 10)
    {
        return Ok(await _analyticsService.GetTopTagsAsync(from, to, limit));
    }

    /// <summary>Per-author article count, total views and average time on page.</summary>
    [HttpGet("author-performance")]
    public async Task<IActionResult> GetAuthorPerformance([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        return Ok(await _analyticsService.GetAuthorPerformanceAsync(from, to));
    }

    /// <summary>Per-campaign article count, total views and bounce rate.</summary>
    [HttpGet("campaign-impact")]
    public async Task<IActionResult> GetCampaignImpact([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        return Ok(await _analyticsService.GetCampaignImpactAsync(from, to));
    }
}
