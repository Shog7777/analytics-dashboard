using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.Data;
using AnalyticsDashboard.Api.DTOs.Common;
using AnalyticsDashboard.Api.DTOs.Pageviews;
using AnalyticsDashboard.Api.Models;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsDashboard.Api.Services.Implementations;

public class PageviewService : IPageviewService
{
    private readonly AppDbContext _db;

    public PageviewService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<PageviewDto>> GetAsync(
        int? articleId, DateTime? from, DateTime? to, int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 200 ? 25 : pageSize;

        // Query-string bound DateTimes arrive as Kind=Unspecified; Npgsql only accepts
        // Kind=Utc for 'timestamp with time zone' columns.
        from = from.AsUtc();
        to = to.AsUtc();

        var query = _db.Pageviews.AsQueryable();

        if (articleId.HasValue)
        {
            query = query.Where(p => p.ArticleId == articleId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(p => p.ViewedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(p => p.ViewedAt <= to.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.ViewedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PageviewDto
            {
                Id = p.Id,
                ArticleId = p.ArticleId,
                ArticleTitle = p.Article!.Title,
                ViewedAt = p.ViewedAt,
                DurationSeconds = p.DurationSeconds,
                IsBounce = p.IsBounce
            })
            .ToListAsync();

        return new PagedResult<PageviewDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PageviewDto> CreateAsync(CreatePageviewDto dto)
    {
        var article = await _db.Articles.FindAsync(dto.ArticleId)
            ?? throw ApiException.NotFound(nameof(Article), dto.ArticleId);

        var pageview = new Pageview
        {
            ArticleId = dto.ArticleId,
            ViewedAt = dto.ViewedAt.AsUtc() ?? DateTime.UtcNow,
            DurationSeconds = dto.DurationSeconds,
            IsBounce = dto.IsBounce
        };

        _db.Pageviews.Add(pageview);
        await _db.SaveChangesAsync();

        return new PageviewDto
        {
            Id = pageview.Id,
            ArticleId = pageview.ArticleId,
            ArticleTitle = article.Title,
            ViewedAt = pageview.ViewedAt,
            DurationSeconds = pageview.DurationSeconds,
            IsBounce = pageview.IsBounce
        };
    }

    public async Task DeleteAsync(long id)
    {
        var pageview = await _db.Pageviews.FindAsync(id)
            ?? throw ApiException.NotFound(nameof(Pageview), id);

        _db.Pageviews.Remove(pageview);
        await _db.SaveChangesAsync();
    }
}
