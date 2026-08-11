namespace AnalyticsDashboard.Api.Models;

/// <summary>1-1 extension of Article. ArticleId is both PK and FK (shared primary key).</summary>
public class ArticleDetails
{
    public int ArticleId { get; set; }
    public Article? Article { get; set; }

    public string Summary { get; set; } = string.Empty;
    public string? HeroImageUrl { get; set; }
    public int ReadingTimeSeconds { get; set; }
}
