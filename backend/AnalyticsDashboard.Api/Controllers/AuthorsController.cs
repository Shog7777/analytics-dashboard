using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.DTOs.Authors;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsDashboard.Api.Controllers;

[ApiController]
[Route("api/authors")]
[Authorize(Roles = Roles.AnyRole)]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;
    public AuthorsController(IAuthorService authorService) => _authorService = authorService;

    [HttpGet]
    public async Task<ActionResult<List<AuthorDto>>> GetAll() => Ok(await _authorService.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = Roles.EditorOrAdmin)]
    public async Task<ActionResult<AuthorDto>> Create(CreateAuthorDto dto) => Ok(await _authorService.CreateAsync(dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        await _authorService.DeleteAsync(id);
        return NoContent();
    }
}
