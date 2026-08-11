using AnalyticsDashboard.Api.DTOs.Auth;

namespace AnalyticsDashboard.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<List<UserDto>> GetUsersAsync();
    Task<UserDto> UpdateUserRoleAsync(int userId, string role);
}
