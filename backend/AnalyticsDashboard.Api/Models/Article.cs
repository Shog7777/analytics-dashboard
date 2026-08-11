namespace AnalyticsDashboard.Api.Models;

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // 1-1
    public ArticleDetails? Details { get; set; }

    // 1-N
    public List<Pageview> Pageviews { get; set; } = new();

    // N-N junction tables
    public List<ArticleTag> ArticleTags { get; set; } = new();
    public List<ArticleAuthor> ArticleAuthors { get; set; } = new();
    public List<ArticleCampaign> ArticleCampaigns { get; set; } = new();
}
