using AnalyticsDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnalyticsDashboard.Api.Data.Configurations;

public class ArticleDetailsConfiguration : IEntityTypeConfiguration<ArticleDetails>
{
    public void Configure(EntityTypeBuilder<ArticleDetails> builder)
    {
        builder.ToTable("article_details");

        // Shared primary key: article_details.article_id is both PK and FK to articles.id
        builder.HasKey(d => d.ArticleId);

        builder.Property(d => d.Summary)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(d => d.HeroImageUrl).HasMaxLength(500);
        builder.Property(d => d.ReadingTimeSeconds).IsRequired();
    }
}
