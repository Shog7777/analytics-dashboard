using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.Data;
using AnalyticsDashboard.Api.DTOs.Analytics;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsDashboard.Api.Services.Implementations;

public class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _db;

    public AnalyticsService(AppDbContext db)
    {
        _db = db;
    }

    private class ArticleStat
    {
        public int ArticleId { get; set; }
        public long Views { get; set; }
        public long DurationSum { get; set; }
        public long BounceCount { get; set; }
    }

    public async Task<KpiSummaryDto> GetKpisAsync(DateTime? from, DateTime? to)
    {
        var (start, end) = await ResolveRangeAsync(from, to);

        var query = _db.Pageviews.Where(p => p.ViewedAt >= start && p.ViewedAt <= end);

        var totalViews = await query.LongCountAsync();

        if (totalViews == 0)
        {
            return new KpiSummaryDto(0, 0, 0, start, end);
        }

        var avgDuration = await query.AverageAsync(p => (double)p.DurationSeconds);
        var bounceCount = await query.CountAsync(p => p.IsBounce);
        var bounceRate = bounceCount * 100.0 / totalViews;

        return new KpiSummaryDto(totalViews, Math.Round(avgDuration, 1), Math.Round(bounceRate, 2), start, end);
    }

    public async Task<List<DailyViewsPointDto>> GetDailyViewsAsync(DateTime? from, DateTime? to)
    {
        var (start, end) = await ResolveRangeAsync(from, to);

        var grouped = await _db.Pageviews
            .Where(p => p.ViewedAt >= start && p.ViewedAt <= end)
            .GroupBy(p => p.ViewedAt.Date)
            .Select(g => new { Date = g.Key, Views = g.LongCount() })
            .OrderBy(g => g.Date)
            .ToListAsync();

        return grouped
            .Select(g => new DailyViewsPointDto(DateOnly.FromDateTime(g.Date), g.Views))
            .ToList();
    }

    public async Task<List<TopArticleDto>> GetTopArticlesAsync(DateTime? from, DateTime? to, int limit)
    {
        var (start, end) = await ResolveRangeAsync(from, to);
        limit = limit is < 1 or > 100 ? 10 : limit;

        // Round after ToListAsync() - Math.Round(double, int) doesn't translate to SQL.
        var raw = await _db.Pageviews
            .Where(p => p.ViewedAt >= start && p.ViewedAt <= end)
            .GroupBy(p => new { p.ArticleId, p.Article!.Title, p.Article.Category })
            .Select(g => new
            {
                g.Key.ArticleId,
                g.Key.Title,
                g.Key.Category,
                Views = g.LongCount(),
                AvgDuration = g.Average(p => (double)p.DurationSeconds)
            })
            .OrderByDescending(a => a.Views)
            .Take(limit)
            .ToListAsync();

        return raw
            .Select(a => new TopArticleDto(a.ArticleId, a.Title, a.Category, a.Views, Math.Round(a.AvgDuration, 1)))
            .ToList();
    }

    public async Task<List<TopTagDto>> GetTopTagsAsync(DateTime? from, DateTime? to, int limit)
    {
        var (start, end) = await ResolveRangeAsync(from, to);
        limit = limit is < 1 or > 100 ? 10 : limit;

        var stats = await GetArticleStatsAsync(start, end);
        var tagArticles = await _db.ArticleTags
            .Select(at => new { at.TagId, TagName = at.Tag!.Name, at.ArticleId })
            .ToListAsync();

        var results = tagArticles
            .GroupBy(x => new { x.TagId, x.TagName })
            .Select(g => new TopTagDto(
                g.Key.TagId,
                g.Key.TagName,
                g.Sum(x => stats.TryGetValue(x.ArticleId, out var s) ? s.Views : 0),
                g.Select(x => x.ArticleId).Distinct().Count()))
            .OrderByDescending(t => t.Views)
            .Take(limit)
            .ToList();

        return results;
    }

    public async Task<List<AuthorPerformanceDto>> GetAuthorPerformanceAsync(DateTime? from, DateTime? to)
    {
        var (start, end) = await ResolveRangeAsync(from, to);
        var stats = await GetArticleStatsAsync(start, end);

        var authors = await _db.Authors
            .Select(author => new
            {
                author.Id,
                author.Name,
                ArticleIds = author.ArticleAuthors.Select(aa => aa.ArticleId).ToList()
            })
            .ToListAsync();

        var results = authors.Select(author =>
        {
            var relevant = author.ArticleIds
                .Where(stats.ContainsKey)
                .Select(id => stats[id])
                .ToList();

            var totalViews = relevant.Sum(s => s.Views);
            var avgDuration = totalViews == 0 ? 0 : relevant.Sum(s => s.DurationSum) / (double)totalViews;

            return new AuthorPerformanceDto(author.Id, author.Name, author.ArticleIds.Count, totalViews, Math.Round(avgDuration, 1));
        })
        .OrderByDescending(r => r.TotalViews)
        .ToList();

        return results;
    }

    public async Task<List<CampaignImpactDto>> GetCampaignImpactAsync(DateTime? from, DateTime? to)
    {
        var (start, end) = await ResolveRangeAsync(from, to);
        var stats = await GetArticleStatsAsync(start, end);

        var campaigns = await _db.Campaigns
            .Select(campaign => new
            {
                campaign.Id,
                campaign.Name,
                ArticleIds = campaign.ArticleCampaigns.Select(ac => ac.ArticleId).ToList()
            })
            .ToListAsync();

        var results = campaigns.Select(campaign =>
        {
            var relevant = campaign.ArticleIds
                .Where(stats.ContainsKey)
                .Select(id => stats[id])
                .ToList();

            var totalViews = relevant.Sum(s => s.Views);
            var bounceCount = relevant.Sum(s => s.BounceCount);
            var bounceRate = totalViews == 0 ? 0 : bounceCount * 100.0 / totalViews;

            return new CampaignImpactDto(campaign.Id, campaign.Name, campaign.ArticleIds.Count, totalViews, Math.Round(bounceRate, 2));
        })
        .OrderByDescending(r => r.TotalViews)
        .ToList();

        return results;
    }

    /// <summary>Per-article views/duration/bounces for the period, shared by top-tags, author-performance and campaign-impact so they can join in memory instead of running separate subqueries.</summary>
    private async Task<Dictionary<int, ArticleStat>> GetArticleStatsAsync(DateTime start, DateTime end)
    {
        var stats = await _db.Pageviews
            .Where(p => p.ViewedAt >= start && p.ViewedAt <= end)
            .GroupBy(p => p.ArticleId)
            .Select(g => new ArticleStat
            {
                ArticleId = g.Key,
                Views = g.LongCount(),
                DurationSum = g.Sum(p => (long)p.DurationSeconds),
                BounceCount = g.Count(p => p.IsBounce)
            })
            .ToListAsync();

        return stats.ToDictionary(s => s.ArticleId);
    }

    /// <summary>Defaults to the full range of recorded pageviews when from/to aren't given.</summary>
    private async Task<(DateTime start, DateTime end)> ResolveRangeAsync(DateTime? from, DateTime? to)
    {
        // Normalize to UTC - query-string dates come in as Kind=Unspecified, which Npgsql rejects.
        from = from.AsUtc();
        to = to.AsUtc();

        if (from.HasValue && to.HasValue)
        {
            return (from.Value, to.Value);
        }

        var hasAny = await _db.Pageviews.AnyAsync();
        if (!hasAny)
        {
            var now = DateTime.UtcNow;
            return (from ?? now.AddDays(-30), to ?? now);
        }

        var minDate = from ?? await _db.Pageviews.MinAsync(p => p.ViewedAt);
        var maxDate = to ?? await _db.Pageviews.MaxAsync(p => p.ViewedAt);
        return (minDate.AsUtc(), maxDate.AsUtc());
    }
}
