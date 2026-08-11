using System.ComponentModel.DataAnnotations;

namespace AnalyticsDashboard.Api.DTOs.ArticleDetails;

public class ArticleDetailsDto
{
    public int ArticleId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string? HeroImageUrl { get; set; }
    public int ReadingTimeSeconds { get; set; }
}

public class UpsertArticleDetailsDto
{
    [Required, StringLength(2000, MinimumLength = 1)]
    public string Summary { get; set; } = string.Empty;

    [StringLength(500)]
    public string? HeroImageUrl { get; set; }

    [Range(0, int.MaxValue)]
    public int ReadingTimeSeconds { get; set; }
}
