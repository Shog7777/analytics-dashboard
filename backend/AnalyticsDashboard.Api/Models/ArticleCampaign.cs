namespace AnalyticsDashboard.Api.Models;

/// <summary>Junction table: Article (N) &lt;-&gt; (N) Campaign.</summary>
public class ArticleCampaign
{
    public int ArticleId { get; set; }
    public Article? Article { get; set; }

    public int CampaignId { get; set; }
    public Campaign? Campaign { get; set; }
}
