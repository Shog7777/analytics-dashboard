using AnalyticsDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnalyticsDashboard.Api.Data.Configurations;

public class PageviewConfiguration : IEntityTypeConfiguration<Pageview>
{
    public void Configure(EntityTypeBuilder<Pageview> builder)
    {
        builder.ToTable("pageviews");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ViewedAt).IsRequired();
        builder.Property(p => p.DurationSeconds).IsRequired();
        builder.Property(p => p.IsBounce).IsRequired();

        // Composite index: this is the hot query path (KPIs, daily views, recent pageviews
        // table) which always filters by article and/or a date range.
        builder.HasIndex(p => new { p.ArticleId, p.ViewedAt });
        builder.HasIndex(p => p.ViewedAt);
    }
}
