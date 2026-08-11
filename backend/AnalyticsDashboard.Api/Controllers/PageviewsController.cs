using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.DTOs.Pageviews;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsDashboard.Api.Controllers;

[ApiController]
[Route("api/pageviews")]
public class PageviewsController : ControllerBase
{
    private readonly IPageviewService _pageviewService;

    public PageviewsController(IPageviewService pageviewService)
    {
        _pageviewService = pageviewService;
    }

    /// <summary>Recent pageviews table, filterable by article and date range.</summary>
    [HttpGet]
    [Authorize(Roles = Roles.AnyRole)]
    public async Task<IActionResult> Get(
        [FromQuery] int? articleId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        return Ok(await _pageviewService.GetAsync(articleId, from, to, page, pageSize));
    }

    /// <summary>Logs one pageview. Anonymous because it's called from the public article page, not the dashboard.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PageviewDto>> Create(CreatePageviewDto dto)
    {
        var created = await _pageviewService.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { articleId = created.ArticleId }, created);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(long id)
    {
        await _pageviewService.DeleteAsync(id);
        return NoContent();
    }
}
