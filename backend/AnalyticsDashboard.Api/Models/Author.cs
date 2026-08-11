namespace AnalyticsDashboard.Api.Models;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }

    public List<ArticleAuthor> ArticleAuthors { get; set; } = new();
}
