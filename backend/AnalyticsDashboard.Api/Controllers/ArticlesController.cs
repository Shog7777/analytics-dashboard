using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.DTOs.ArticleDetails;
using AnalyticsDashboard.Api.DTOs.Articles;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsDashboard.Api.Controllers;

[ApiController]
[Route("api/articles")]
[Authorize(Roles = Roles.AnyRole)]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    /// <summary>List/search articles with optional filters and pagination.</summary>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] int? tagId,
        [FromQuery] int? authorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _articleService.SearchAsync(search, category, tagId, authorId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ArticleDto>> GetById(int id)
    {
        return Ok(await _articleService.GetByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Roles = Roles.EditorOrAdmin)]
    public async Task<ActionResult<ArticleDto>> Create(CreateArticleDto dto)
    {
        var created = await _articleService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.EditorOrAdmin)]
    public async Task<ActionResult<ArticleDto>> Update(int id, UpdateArticleDto dto)
    {
        return Ok(await _articleService.UpdateAsync(id, dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        await _articleService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>Create or replace the 1-1 article_details record for this article.</summary>
    [HttpPut("{id:int}/details")]
    [Authorize(Roles = Roles.EditorOrAdmin)]
    public async Task<ActionResult<ArticleDetailsDto>> UpsertDetails(int id, UpsertArticleDetailsDto dto)
    {
        return Ok(await _articleService.UpsertDetailsAsync(id, dto));
    }

    /// <summary>Replaces the article's tag associations with the given set of tag ids.</summary>
    [HttpPut("{id:int}/tags")]
    [Authorize(Roles = Roles.EditorOrAdmin)]
    public async Task<IActionResult> SetTags(int id, UpdateArticleAssociationsDto dto)
    {
        return Ok(await _articleService.SetTagsAsync(id, dto.Ids));
    }

    /// <summary>Replaces the article's author associations with the given set of author ids.</summary>
    [HttpPut("{id:int}/authors")]
    [Authorize(Roles = Roles.EditorOrAdmin)]
    public async Task<IActionResult> SetAuthors(int id, UpdateArticleAssociationsDto dto)
    {
        return Ok(await _articleService.SetAuthorsAsync(id, dto.Ids));
    }

    /// <summary>Replaces the article's campaign associations with the given set of campaign ids.</summary>
    [HttpPut("{id:int}/campaigns")]
    [Authorize(Roles = Roles.EditorOrAdmin)]
    public async Task<IActionResult> SetCampaigns(int id, UpdateArticleAssociationsDto dto)
    {
        return Ok(await _articleService.SetCampaignsAsync(id, dto.Ids));
    }
}
