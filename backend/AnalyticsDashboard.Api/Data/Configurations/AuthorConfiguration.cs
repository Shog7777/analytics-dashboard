using AnalyticsDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnalyticsDashboard.Api.Data.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("authors");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Email).IsRequired().HasMaxLength(300);
        builder.Property(a => a.Bio).HasMaxLength(1000);
        builder.HasIndex(a => a.Email).IsUnique();
    }
}

public class ArticleAuthorConfiguration : IEntityTypeConfiguration<ArticleAuthor>
{
    public void Configure(EntityTypeBuilder<ArticleAuthor> builder)
    {
        builder.ToTable("article_authors");
        builder.HasKey(aa => new { aa.ArticleId, aa.AuthorId });

        builder.HasOne(aa => aa.Article)
            .WithMany(a => a.ArticleAuthors)
            .HasForeignKey(aa => aa.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(aa => aa.Author)
            .WithMany(a => a.ArticleAuthors)
            .HasForeignKey(aa => aa.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
