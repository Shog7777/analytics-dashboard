using AnalyticsDashboard.Api.DTOs.ArticleDetails;
using AnalyticsDashboard.Api.DTOs.Articles;
using AnalyticsDashboard.Api.DTOs.Common;

namespace AnalyticsDashboard.Api.Services.Interfaces;

public interface IArticleService
{
    Task<PagedResult<ArticleListItemDto>> SearchAsync(
        string? search, string? category, int? tagId, int? authorId, int page, int pageSize);

    Task<ArticleDto> GetByIdAsync(int id);
    Task<ArticleDto> CreateAsync(CreateArticleDto dto);
    Task<ArticleDto> UpdateAsync(int id, UpdateArticleDto dto);
    Task DeleteAsync(int id);

    Task<ArticleDetailsDto> UpsertDetailsAsync(int articleId, UpsertArticleDetailsDto dto);

    Task<List<TagRefDto>> SetTagsAsync(int articleId, List<int> tagIds);
    Task<List<AuthorRefDto>> SetAuthorsAsync(int articleId, List<int> authorIds);
    Task<List<CampaignRefDto>> SetCampaignsAsync(int articleId, List<int> campaignIds);
}
