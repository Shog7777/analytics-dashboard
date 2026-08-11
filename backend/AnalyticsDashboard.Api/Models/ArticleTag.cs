namespace AnalyticsDashboard.Api.Models;

/// <summary>Junction table: Article (N) &lt;-&gt; (N) Tag.</summary>
public class ArticleTag
{
    public int ArticleId { get; set; }
    public Article? Article { get; set; }

    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}
