using AnalyticsDashboard.Api.DTOs.Authors;
using AnalyticsDashboard.Api.DTOs.Campaigns;
using AnalyticsDashboard.Api.DTOs.Tags;

namespace AnalyticsDashboard.Api.Services.Interfaces;

public interface ITagService
{
    Task<List<TagDto>> GetAllAsync();
    Task<TagDto> CreateAsync(CreateTagDto dto);
    Task DeleteAsync(int id);
}

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAllAsync();
    Task<AuthorDto> CreateAsync(CreateAuthorDto dto);
    Task DeleteAsync(int id);
}

public interface ICampaignService
{
    Task<List<CampaignDto>> GetAllAsync();
    Task<CampaignDto> CreateAsync(CreateCampaignDto dto);
    Task DeleteAsync(int id);
}
