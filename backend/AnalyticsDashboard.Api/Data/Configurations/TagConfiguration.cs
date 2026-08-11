using AnalyticsDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnalyticsDashboard.Api.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(t => t.Name).IsUnique();
    }
}

public class ArticleTagConfiguration : IEntityTypeConfiguration<ArticleTag>
{
    public void Configure(EntityTypeBuilder<ArticleTag> builder)
    {
        builder.ToTable("article_tags");
        builder.HasKey(at => new { at.ArticleId, at.TagId });

        builder.HasOne(at => at.Article)
            .WithMany(a => a.ArticleTags)
            .HasForeignKey(at => at.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(at => at.Tag)
            .WithMany(t => t.ArticleTags)
            .HasForeignKey(at => at.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
