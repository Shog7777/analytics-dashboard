using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.Data;
using AnalyticsDashboard.Api.DTOs.ArticleDetails;
using AnalyticsDashboard.Api.DTOs.Articles;
using AnalyticsDashboard.Api.DTOs.Common;
using AnalyticsDashboard.Api.Models;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsDashboard.Api.Services.Implementations;

public class ArticleService : IArticleService
{
    private readonly AppDbContext _db;

    public ArticleService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<ArticleListItemDto>> SearchAsync(
        string? search, string? category, int? tagId, int? authorId, int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var query = _db.Articles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(a => a.Title.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(a => a.Category == category);
        }

        if (tagId.HasValue)
        {
            query = query.Where(a => a.ArticleTags.Any(at => at.TagId == tagId.Value));
        }

        if (authorId.HasValue)
        {
            query = query.Where(a => a.ArticleAuthors.Any(aa => aa.AuthorId == authorId.Value));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ArticleListItemDto
            {
                Id = a.Id,
                Title = a.Title,
                Category = a.Category,
                PublishedAt = a.PublishedAt,
                Tags = a.ArticleTags.Select(at => at.Tag!.Name).ToList(),
                Authors = a.ArticleAuthors.Select(aa => aa.Author!.Name).ToList()
            })
            .ToListAsync();

        return new PagedResult<ArticleListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ArticleDto> GetByIdAsync(int id)
    {
        var article = await _db.Articles
            .Include(a => a.Details)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .Include(a => a.ArticleAuthors).ThenInclude(aa => aa.Author)
            .Include(a => a.ArticleCampaigns).ThenInclude(ac => ac.Campaign)
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw ApiException.NotFound(nameof(Article), id);

        return MapToDto(article);
    }

    public async Task<ArticleDto> CreateAsync(CreateArticleDto dto)
    {
        var article = new Article
        {
            Title = dto.Title,
            Category = dto.Category,
            PublishedAt = dto.PublishedAt.AsUtc(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Articles.Add(article);
        await _db.SaveChangesAsync();

        if (dto.TagIds is { Count: > 0 })
        {
            await SetTagsAsync(article.Id, dto.TagIds);
        }

        if (dto.AuthorIds is { Count: > 0 })
        {
            await SetAuthorsAsync(article.Id, dto.AuthorIds);
        }

        if (dto.CampaignIds is { Count: > 0 })
        {
            await SetCampaignsAsync(article.Id, dto.CampaignIds);
        }

        return await GetByIdAsync(article.Id);
    }

    public async Task<ArticleDto> UpdateAsync(int id, UpdateArticleDto dto)
    {
        var article = await _db.Articles.FindAsync(id)
            ?? throw ApiException.NotFound(nameof(Article), id);

        article.Title = dto.Title;
        article.Category = dto.Category;
        article.PublishedAt = dto.PublishedAt.AsUtc();
        article.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var article = await _db.Articles.FindAsync(id)
            ?? throw ApiException.NotFound(nameof(Article), id);

        // Cascade delete (configured in ArticleConfiguration/PageviewConfiguration) removes
        // the article's details, pageviews and junction rows automatically.
        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();
    }

    public async Task<ArticleDetailsDto> UpsertDetailsAsync(int articleId, UpsertArticleDetailsDto dto)
    {
        var articleExists = await _db.Articles.AnyAsync(a => a.Id == articleId);
        if (!articleExists)
        {
            throw ApiException.NotFound(nameof(Article), articleId);
        }

        var details = await _db.ArticleDetails.FindAsync(articleId);

        if (details is null)
        {
            details = new ArticleDetails { ArticleId = articleId };
            _db.ArticleDetails.Add(details);
        }

        details.Summary = dto.Summary;
        details.HeroImageUrl = dto.HeroImageUrl;
        details.ReadingTimeSeconds = dto.ReadingTimeSeconds;

        await _db.SaveChangesAsync();

        return new ArticleDetailsDto
        {
            ArticleId = details.ArticleId,
            Summary = details.Summary,
            HeroImageUrl = details.HeroImageUrl,
            ReadingTimeSeconds = details.ReadingTimeSeconds
        };
    }

    public async Task<List<TagRefDto>> SetTagsAsync(int articleId, List<int> tagIds)
    {
        await EnsureArticleExists(articleId);

        var validTagIds = await _db.Tags.Where(t => tagIds.Contains(t.Id)).Select(t => t.Id).ToListAsync();

        var existing = _db.ArticleTags.Where(at => at.ArticleId == articleId);
        _db.ArticleTags.RemoveRange(existing);

        foreach (var tagId in validTagIds.Distinct())
        {
            _db.ArticleTags.Add(new ArticleTag { ArticleId = articleId, TagId = tagId });
        }

        await _db.SaveChangesAsync();

        return await _db.ArticleTags
            .Where(at => at.ArticleId == articleId)
            .Select(at => new TagRefDto(at.TagId, at.Tag!.Name))
            .ToListAsync();
    }

    public async Task<List<AuthorRefDto>> SetAuthorsAsync(int articleId, List<int> authorIds)
    {
        await EnsureArticleExists(articleId);

        var validAuthorIds = await _db.Authors.Where(a => authorIds.Contains(a.Id)).Select(a => a.Id).ToListAsync();

        var existing = _db.ArticleAuthors.Where(aa => aa.ArticleId == articleId);
        _db.ArticleAuthors.RemoveRange(existing);

        foreach (var authorId in validAuthorIds.Distinct())
        {
            _db.ArticleAuthors.Add(new ArticleAuthor { ArticleId = articleId, AuthorId = authorId });
        }

        await _db.SaveChangesAsync();

        return await _db.ArticleAuthors
            .Where(aa => aa.ArticleId == articleId)
            .Select(aa => new AuthorRefDto(aa.AuthorId, aa.Author!.Name))
            .ToListAsync();
    }

    public async Task<List<CampaignRefDto>> SetCampaignsAsync(int articleId, List<int> campaignIds)
    {
        await EnsureArticleExists(articleId);

        var validCampaignIds = await _db.Campaigns.Where(c => campaignIds.Contains(c.Id)).Select(c => c.Id).ToListAsync();

        var existing = _db.ArticleCampaigns.Where(ac => ac.ArticleId == articleId);
        _db.ArticleCampaigns.RemoveRange(existing);

        foreach (var campaignId in validCampaignIds.Distinct())
        {
            _db.ArticleCampaigns.Add(new ArticleCampaign { ArticleId = articleId, CampaignId = campaignId });
        }

        await _db.SaveChangesAsync();

        return await _db.ArticleCampaigns
            .Where(ac => ac.ArticleId == articleId)
            .Select(ac => new CampaignRefDto(ac.CampaignId, ac.Campaign!.Name))
            .ToListAsync();
    }

    private async Task EnsureArticleExists(int articleId)
    {
        var exists = await _db.Articles.AnyAsync(a => a.Id == articleId);
        if (!exists)
        {
            throw ApiException.NotFound(nameof(Article), articleId);
        }
    }

    private static ArticleDto MapToDto(Article article)
    {
        return new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Category = article.Category,
            PublishedAt = article.PublishedAt,
            CreatedAt = article.CreatedAt,
            UpdatedAt = article.UpdatedAt,
            Details = article.Details is null ? null : new ArticleDetailsDto
            {
                ArticleId = article.Details.ArticleId,
                Summary = article.Details.Summary,
                HeroImageUrl = article.Details.HeroImageUrl,
                ReadingTimeSeconds = article.Details.ReadingTimeSeconds
            },
            Tags = article.ArticleTags.Select(at => new TagRefDto(at.TagId, at.Tag!.Name)).ToList(),
            Authors = article.ArticleAuthors.Select(aa => new AuthorRefDto(aa.AuthorId, aa.Author!.Name)).ToList(),
            Campaigns = article.ArticleCampaigns.Select(ac => new CampaignRefDto(ac.CampaignId, ac.Campaign!.Name)).ToList()
        };
    }
}
