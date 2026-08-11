using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.DTOs.Tags;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsDashboard.Api.Controllers;

[ApiController]
[Route("api/tags")]
[Authorize(Roles = Roles.AnyRole)]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;
    public TagsController(ITagService tagService) => _tagService = tagService;

    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetAll() => Ok(await _tagService.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = Roles.EditorOrAdmin)]
    public async Task<ActionResult<TagDto>> Create(CreateTagDto dto) => Ok(await _tagService.CreateAsync(dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        await _tagService.DeleteAsync(id);
        return NoContent();
    }
}
