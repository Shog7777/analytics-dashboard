using System.ComponentModel.DataAnnotations;

namespace AnalyticsDashboard.Api.DTOs.Tags;

public record TagDto(int Id, string Name);

public class CreateTagDto
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
}
