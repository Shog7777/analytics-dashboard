namespace AnalyticsDashboard.Api.DTOs.Analytics;

public record KpiSummaryDto(
    long TotalViews,
    double AvgTimeOnPageSeconds,
    double BounceRatePercent,
    DateTime PeriodStart,
    DateTime PeriodEnd);

public record DailyViewsPointDto(DateOnly Date, long Views);

public record TopArticleDto(int ArticleId, string Title, string Category, long Views, double AvgTimeOnPageSeconds);

public record TopTagDto(int TagId, string Name, long Views, int ArticleCount);

public record AuthorPerformanceDto(int AuthorId, string Name, int ArticleCount, long TotalViews, double AvgTimeOnPageSeconds);

public record CampaignImpactDto(int CampaignId, string Name, int ArticleCount, long TotalViews, double BounceRatePercent);
