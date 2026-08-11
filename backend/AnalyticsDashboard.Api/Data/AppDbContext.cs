using AnalyticsDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsDashboard.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<ArticleDetails> ArticleDetails => Set<ArticleDetails>();
    public DbSet<Pageview> Pageviews => Set<Pageview>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ArticleTag> ArticleTags => Set<ArticleTag>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<ArticleAuthor> ArticleAuthors => Set<ArticleAuthor>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<ArticleCampaign> ArticleCampaigns => Set<ArticleCampaign>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Applies every IEntityTypeConfiguration<T> found in this assembly
        // (see Data/Configurations/*.cs) instead of configuring everything inline here.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
