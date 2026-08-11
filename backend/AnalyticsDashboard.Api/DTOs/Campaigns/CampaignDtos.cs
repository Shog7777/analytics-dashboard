using System.ComponentModel.DataAnnotations;

namespace AnalyticsDashboard.Api.DTOs.Campaigns;

public record CampaignDto(int Id, string Name, string? Description, DateTime StartDate, DateTime? EndDate);

public class CreateCampaignDto
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
