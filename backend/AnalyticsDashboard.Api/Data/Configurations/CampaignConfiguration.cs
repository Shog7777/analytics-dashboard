using AnalyticsDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnalyticsDashboard.Api.Data.Configurations;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("campaigns");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).HasMaxLength(1000);
        builder.Property(c => c.StartDate).IsRequired();
    }
}

public class ArticleCampaignConfiguration : IEntityTypeConfiguration<ArticleCampaign>
{
    public void Configure(EntityTypeBuilder<ArticleCampaign> builder)
    {
        builder.ToTable("article_campaigns");
        builder.HasKey(ac => new { ac.ArticleId, ac.CampaignId });

        builder.HasOne(ac => ac.Article)
            .WithMany(a => a.ArticleCampaigns)
            .HasForeignKey(ac => ac.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ac => ac.Campaign)
            .WithMany(c => c.ArticleCampaigns)
            .HasForeignKey(ac => ac.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
