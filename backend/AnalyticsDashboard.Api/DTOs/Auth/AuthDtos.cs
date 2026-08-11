using System.ComponentModel.DataAnnotations;

namespace AnalyticsDashboard.Api.DTOs.Auth;

public class RegisterDto
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(300)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public record UserDto(int Id, string Username, string Email, string Role);

public record AuthResponseDto(string Token, DateTime ExpiresAt, UserDto User);

public class UpdateUserRoleDto
{
    [Required]
    public string Role { get; set; } = string.Empty;
}
