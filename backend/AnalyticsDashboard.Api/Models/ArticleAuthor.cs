namespace AnalyticsDashboard.Api.Models;

/// <summary>Junction table: Article (N) &lt;-&gt; (N) Author (an article can be co-authored).</summary>
public class ArticleAuthor
{
    public int ArticleId { get; set; }
    public Article? Article { get; set; }

    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}
