using System.ComponentModel.DataAnnotations;
using AnalyticsDashboard.Api.DTOs.ArticleDetails;

namespace AnalyticsDashboard.Api.DTOs.Articles;

/// <summary>Lightweight shape used in list/search results.</summary>
public class ArticleListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Authors { get; set; } = new();
}

/// <summary>Full shape used for the article detail page.</summary>
public class ArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ArticleDetailsDto? Details { get; set; }

    public List<TagRefDto> Tags { get; set; } = new();
    public List<AuthorRefDto> Authors { get; set; } = new();
    public List<CampaignRefDto> Campaigns { get; set; } = new();
}

public record TagRefDto(int Id, string Name);
public record AuthorRefDto(int Id, string Name);
public record CampaignRefDto(int Id, string Name);

public class CreateArticleDto
{
    [Required, StringLength(300, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public DateTime PublishedAt { get; set; }

    /// <summary>Optional: attach existing tag/author/campaign ids at creation time.</summary>
    public List<int>? TagIds { get; set; }
    public List<int>? AuthorIds { get; set; }
    public List<int>? CampaignIds { get; set; }
}

public class UpdateArticleDto
{
    [Required, StringLength(300, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public DateTime PublishedAt { get; set; }
}

/// <summary>Replaces the full set of tag/author/campaign associations for an article.</summary>
public class UpdateArticleAssociationsDto
{
    public List<int> Ids { get; set; } = new();
}
