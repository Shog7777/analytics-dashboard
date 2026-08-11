namespace AnalyticsDashboard.Api.Models;

public class Pageview
{
    public long Id { get; set; }
    public int ArticleId { get; set; }
    public Article? Article { get; set; }

    public DateTime ViewedAt { get; set; }
    public int DurationSeconds { get; set; }
    public bool IsBounce { get; set; }
}
