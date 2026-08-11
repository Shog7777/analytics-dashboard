using System.ComponentModel.DataAnnotations;

namespace AnalyticsDashboard.Api.DTOs.Pageviews;

public class PageviewDto
{
    public long Id { get; set; }
    public int ArticleId { get; set; }
    public string ArticleTitle { get; set; } = string.Empty;
    public DateTime ViewedAt { get; set; }
    public int DurationSeconds { get; set; }
    public bool IsBounce { get; set; }
}

public class CreatePageviewDto
{
    [Required]
    public int ArticleId { get; set; }

    /// <summary>Defaults to UtcNow when omitted (e.g. real-time tracking beacon).</summary>
    public DateTime? ViewedAt { get; set; }

    [Range(0, int.MaxValue)]
    public int DurationSeconds { get; set; }

    public bool IsBounce { get; set; }
}
