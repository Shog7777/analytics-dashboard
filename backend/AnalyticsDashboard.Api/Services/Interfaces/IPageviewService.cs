using AnalyticsDashboard.Api.DTOs.Common;
using AnalyticsDashboard.Api.DTOs.Pageviews;

namespace AnalyticsDashboard.Api.Services.Interfaces;

public interface IPageviewService
{
    Task<PagedResult<PageviewDto>> GetAsync(
        int? articleId, DateTime? from, DateTime? to, int page, int pageSize);

    Task<PageviewDto> CreateAsync(CreatePageviewDto dto);
    Task DeleteAsync(long id);
}
