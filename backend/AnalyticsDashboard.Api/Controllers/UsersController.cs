using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.DTOs.Auth;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsDashboard.Api.Controllers;

/// <summary>Admin-only user & role management.</summary>
[ApiController]
[Route("api/users")]
[Authorize(Roles = Roles.Admin)]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        return Ok(await _authService.GetUsersAsync());
    }

    [HttpPut("{id:int}/role")]
    public async Task<ActionResult<UserDto>> UpdateRole(int id, UpdateUserRoleDto dto)
    {
        return Ok(await _authService.UpdateUserRoleAsync(id, dto.Role));
    }
}
