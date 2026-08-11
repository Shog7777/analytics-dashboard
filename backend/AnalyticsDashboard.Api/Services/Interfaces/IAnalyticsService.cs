using AnalyticsDashboard.Api.DTOs.Analytics;

namespace AnalyticsDashboard.Api.Services.Interfaces;

public interface IAnalyticsService
{
    Task<KpiSummaryDto> GetKpisAsync(DateTime? from, DateTime? to);
    Task<List<DailyViewsPointDto>> GetDailyViewsAsync(DateTime? from, DateTime? to);
    Task<List<TopArticleDto>> GetTopArticlesAsync(DateTime? from, DateTime? to, int limit);
    Task<List<TopTagDto>> GetTopTagsAsync(DateTime? from, DateTime? to, int limit);
    Task<List<AuthorPerformanceDto>> GetAuthorPerformanceAsync(DateTime? from, DateTime? to);
    Task<List<CampaignImpactDto>> GetCampaignImpactAsync(DateTime? from, DateTime? to);
}
