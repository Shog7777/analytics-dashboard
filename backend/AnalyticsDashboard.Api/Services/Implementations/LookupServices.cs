using AnalyticsDashboard.Api.Common;
using AnalyticsDashboard.Api.Data;
using AnalyticsDashboard.Api.DTOs.Authors;
using AnalyticsDashboard.Api.DTOs.Campaigns;
using AnalyticsDashboard.Api.DTOs.Tags;
using AnalyticsDashboard.Api.Models;
using AnalyticsDashboard.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsDashboard.Api.Services.Implementations;

public class TagService : ITagService
{
    private readonly AppDbContext _db;
    public TagService(AppDbContext db) => _db = db;

    public async Task<List<TagDto>> GetAllAsync() =>
        await _db.Tags.OrderBy(t => t.Name).Select(t => new TagDto(t.Id, t.Name)).ToListAsync();

    public async Task<TagDto> CreateAsync(CreateTagDto dto)
    {
        var normalized = dto.Name.Trim();
        var exists = await _db.Tags.AnyAsync(t => t.Name.ToLower() == normalized.ToLower());
        if (exists)
        {
            throw ApiException.Conflict($"Tag '{normalized}' already exists.");
        }

        var tag = new Tag { Name = normalized };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();
        return new TagDto(tag.Id, tag.Name);
    }

    public async Task DeleteAsync(int id)
    {
        var tag = await _db.Tags.FindAsync(id) ?? throw ApiException.NotFound(nameof(Tag), id);
        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync();
    }
}

public class AuthorService : IAuthorService
{
    private readonly AppDbContext _db;
    public AuthorService(AppDbContext db) => _db = db;

    public async Task<List<AuthorDto>> GetAllAsync() =>
        await _db.Authors.OrderBy(a => a.Name)
            .Select(a => new AuthorDto(a.Id, a.Name, a.Email, a.Bio))
            .ToListAsync();

    public async Task<AuthorDto> CreateAsync(CreateAuthorDto dto)
    {
        var exists = await _db.Authors.AnyAsync(a => a.Email.ToLower() == dto.Email.ToLower());
        if (exists)
        {
            throw ApiException.Conflict($"Author with email '{dto.Email}' already exists.");
        }

        var author = new Author { Name = dto.Name.Trim(), Email = dto.Email.Trim(), Bio = dto.Bio };
        _db.Authors.Add(author);
        await _db.SaveChangesAsync();
        return new AuthorDto(author.Id, author.Name, author.Email, author.Bio);
    }

    public async Task DeleteAsync(int id)
    {
        var author = await _db.Authors.FindAsync(id) ?? throw ApiException.NotFound(nameof(Author), id);
        _db.Authors.Remove(author);
        await _db.SaveChangesAsync();
    }
}

public class CampaignService : ICampaignService
{
    private readonly AppDbContext _db;
    public CampaignService(AppDbContext db) => _db = db;

    public async Task<List<CampaignDto>> GetAllAsync() =>
        await _db.Campaigns.OrderByDescending(c => c.StartDate)
            .Select(c => new CampaignDto(c.Id, c.Name, c.Description, c.StartDate, c.EndDate))
            .ToListAsync();

    public async Task<CampaignDto> CreateAsync(CreateCampaignDto dto)
    {
        if (dto.EndDate.HasValue && dto.EndDate.Value < dto.StartDate)
        {
            throw ApiException.BadRequest("EndDate cannot be earlier than StartDate.");
        }

        var campaign = new Campaign
        {
            Name = dto.Name.Trim(),
            Description = dto.Description,
            StartDate = dto.StartDate.AsUtc(),
            EndDate = dto.EndDate.AsUtc()
        };

        _db.Campaigns.Add(campaign);
        await _db.SaveChangesAsync();
        return new CampaignDto(campaign.Id, campaign.Name, campaign.Description, campaign.StartDate, campaign.EndDate);
    }

    public async Task DeleteAsync(int id)
    {
        var campaign = await _db.Campaigns.FindAsync(id) ?? throw ApiException.NotFound(nameof(Campaign), id);
        _db.Campaigns.Remove(campaign);
        await _db.SaveChangesAsync();
    }
}
