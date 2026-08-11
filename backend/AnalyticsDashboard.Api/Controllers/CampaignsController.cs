using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.DTOs.Campaigns;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsDashboard.Api.Controllers;

[ApiController]
[Route("api/campaigns")]
[Authorize(Roles = Roles.AnyRole)]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    public CampaignsController(ICampaignService campaignService) => _campaignService = campaignService;

    [HttpGet]
    public async Task<ActionResult<List<CampaignDto>>> GetAll() => Ok(await _campaignService.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = Roles.EditorOrAdmin)]
    public async Task<ActionResult<CampaignDto>> Create(CreateCampaignDto dto) => Ok(await _campaignService.CreateAsync(dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        await _campaignService.DeleteAsync(id);
        return NoContent();
    }
}
