using System.ComponentModel.DataAnnotations;

namespace AnalyticsDashboard.Api.DTOs.Authors;

public record AuthorDto(int Id, string Name, string Email, string? Bio);

public class CreateAuthorDto
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(300)]
    public string Email { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Bio { get; set; }
}
