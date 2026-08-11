using AnalyticsDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnalyticsDashboard.Api.Data.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("articles");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(a => a.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.PublishedAt).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();

        builder.HasIndex(a => a.Category);
        builder.HasIndex(a => a.PublishedAt);

        // 1-1 Article <-> ArticleDetails (shared primary key), configured from the
        // ArticleDetails side as well; declaring it here keeps the relationship explicit.
        builder.HasOne(a => a.Details)
            .WithOne(d => d.Article)
            .HasForeignKey<ArticleDetails>(d => d.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1-N Article -> Pageviews
        builder.HasMany(a => a.Pageviews)
            .WithOne(p => p.Article)
            .HasForeignKey(p => p.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
