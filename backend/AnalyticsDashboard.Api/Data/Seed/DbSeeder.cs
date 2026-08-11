using AnalyticsDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsDashboard.Api.Data.Seed;

/// <summary>Fills an empty database with demo data (articles, tags, authors, campaigns, pageviews, one user per role). Runs once at startup; no-op if data already exists.</summary>
public static class DbSeeder
{
    private static readonly string[] Categories = { "Technology", "Business", "Lifestyle" };

    private static readonly string[] TagNames =
    {
        "AI", "Web Development", "Startups", "Productivity", "Design",
        "Finance", "Health", "Marketing", "Data Science", "Career"
    };

    private static readonly (string Name, string Email)[] AuthorSeed =
    {
        ("Sara Al-Ghamdi", "sara.alghamdi@example.com"),
        ("Omar Haddad", "omar.haddad@example.com"),
        ("Lina Youssef", "lina.youssef@example.com"),
        ("Fahad Al-Otaibi", "fahad.alotaibi@example.com"),
        ("Maya Nasser", "maya.nasser@example.com")
    };

    private static readonly (string Name, string Description)[] CampaignSeed =
    {
        ("Spring Content Push", "Increased publishing cadence and social promotion in Q1."),
        ("Newsletter Relaunch", "Cross-promotion through the revamped weekly newsletter."),
        ("SEO Refresh", "Updated older articles with better titles and metadata.")
    };

    private static readonly string[] TitleTemplates =
    {
        "The Complete Guide to {0}",
        "How {0} Is Changing in 2026",
        "5 Lessons We Learned About {0}",
        "Why {0} Matters More Than Ever",
        "A Beginner's Introduction to {0}",
        "The Future of {0}: What to Expect",
        "Mastering {0} in Under 30 Days",
        "{0} Explained: A Practical Overview",
        "Common Mistakes to Avoid in {0}",
        "Behind the Scenes of {0}"
    };

    private static readonly string[] TitleSubjects =
    {
        "Remote Work", "Cloud Infrastructure", "Personal Finance", "UX Design",
        "Machine Learning", "Content Strategy", "Team Leadership", "E-commerce",
        "Cybersecurity", "Mobile Apps", "Sustainable Business", "Public Speaking",
        "Freelancing", "Product Management", "Digital Marketing", "Data Privacy",
        "Habit Building", "Startup Funding", "API Design", "Customer Retention",
        "Time Management", "Open Source", "Brand Storytelling", "Automation",
        "Nutrition Science", "Career Growth"
    };

    public static async Task SeedAsync(AppDbContext db, IConfiguration config, ILogger logger)
    {
        var seedSection = config.GetSection("Seed");
        var enabled = seedSection.GetValue("EnabledOnStartup", true);
        if (!enabled)
        {
            return;
        }

        if (await db.Articles.AnyAsync())
        {
            logger.LogInformation("Seed skipped: articles table already has data.");
            return;
        }

        var articleCount = Math.Clamp(seedSection.GetValue("ArticleCount", 26), 20, 30);
        var minPageviews = seedSection.GetValue("MinPageviews", 30000);
        var maxPageviews = seedSection.GetValue("MaxPageviews", 60000);
        var daysBack = seedSection.GetValue("DaysBack", 90);

        var random = new Random(20260811); // fixed seed => reproducible demo data
        var now = DateTime.UtcNow;

        logger.LogInformation("Seeding database: {Count} articles, {Min}-{Max} pageviews over {Days} days...",
            articleCount, minPageviews, maxPageviews, daysBack);

        // --- Users (one per role, so grading/demo can log in immediately) ---
        db.Users.AddRange(
            new User { Username = "admin", Email = "admin@analytics-dashboard.local", Role = UserRole.Admin, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123") },
            new User { Username = "editor", Email = "editor@analytics-dashboard.local", Role = UserRole.Editor, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Editor@123") },
            new User { Username = "viewer", Email = "viewer@analytics-dashboard.local", Role = UserRole.Viewer, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Viewer@123") }
        );

        // --- Tags / Authors / Campaigns ---
        var tags = TagNames.Select(name => new Tag { Name = name }).ToList();
        db.Tags.AddRange(tags);

        var authors = AuthorSeed.Select(a => new Author { Name = a.Name, Email = a.Email, Bio = $"{a.Name} writes about {Categories[random.Next(Categories.Length)].ToLower()} topics." }).ToList();
        db.Authors.AddRange(authors);

        var campaigns = CampaignSeed.Select(c => new Campaign
        {
            Name = c.Name,
            Description = c.Description,
            StartDate = now.AddDays(-random.Next(60, daysBack)),
            EndDate = now.AddDays(-random.Next(0, 30))
        }).ToList();
        db.Campaigns.AddRange(campaigns);

        await db.SaveChangesAsync(); // assigns Ids we need for FKs below

        // --- Articles + ArticleDetails + junction rows ---
        var subjects = TitleSubjects.OrderBy(_ => random.Next()).Take(articleCount).ToList();
        var articles = new List<Article>();

        for (var i = 0; i < articleCount; i++)
        {
            var subject = subjects[i % subjects.Count];
            var template = TitleTemplates[random.Next(TitleTemplates.Length)];
            var publishedAt = now.AddDays(-random.Next(1, daysBack));

            var article = new Article
            {
                Title = string.Format(template, subject),
                Category = Categories[random.Next(Categories.Length)],
                PublishedAt = publishedAt,
                CreatedAt = publishedAt
            };
            articles.Add(article);
        }

        db.Articles.AddRange(articles);
        await db.SaveChangesAsync(); // assigns article Ids

        foreach (var article in articles)
        {
            db.ArticleDetails.Add(new ArticleDetails
            {
                ArticleId = article.Id,
                Summary = $"An in-depth look at {article.Title.ToLower()}, covering the key ideas readers need to know.",
                HeroImageUrl = $"https://picsum.photos/seed/article-{article.Id}/800/400",
                ReadingTimeSeconds = random.Next(120, 720)
            });

            // 1-3 tags per article
            foreach (var tag in tags.OrderBy(_ => random.Next()).Take(random.Next(1, 4)))
            {
                db.ArticleTags.Add(new ArticleTag { ArticleId = article.Id, TagId = tag.Id });
            }

            // 1-2 authors per article
            foreach (var author in authors.OrderBy(_ => random.Next()).Take(random.Next(1, 3)))
            {
                db.ArticleAuthors.Add(new ArticleAuthor { ArticleId = article.Id, AuthorId = author.Id });
            }

            // 0-2 campaigns per article
            var campaignCount = random.Next(0, 3);
            if (campaignCount > 0)
            {
                foreach (var campaign in campaigns.OrderBy(_ => random.Next()).Take(campaignCount))
                {
                    db.ArticleCampaigns.Add(new ArticleCampaign { ArticleId = article.Id, CampaignId = campaign.Id });
                }
            }
        }

        await db.SaveChangesAsync();

        // --- Pageviews: skewed distribution so a handful of articles clearly lead the pack ---
        var totalPageviews = random.Next(minPageviews, maxPageviews + 1);
        var weights = articles.Select(_ => random.NextDouble() * random.NextDouble()).ToList(); // right-skewed
        var weightSum = weights.Sum();

        var pageviews = new List<Pageview>(totalPageviews);

        for (var i = 0; i < articles.Count; i++)
        {
            var article = articles[i];
            var share = weights[i] / weightSum;
            var viewsForArticle = Math.Max(5, (int)Math.Round(totalPageviews * share));

            var earliestView = article.PublishedAt > now.AddDays(-daysBack) ? article.PublishedAt : now.AddDays(-daysBack);

            for (var v = 0; v < viewsForArticle; v++)
            {
                var spanMinutes = (int)Math.Max(1, (now - earliestView).TotalMinutes);
                var viewedAt = earliestView.AddMinutes(random.Next(0, spanMinutes));
                var isBounce = random.NextDouble() < 0.35;

                pageviews.Add(new Pageview
                {
                    ArticleId = article.Id,
                    ViewedAt = viewedAt,
                    DurationSeconds = isBounce ? random.Next(3, 20) : random.Next(30, 480),
                    IsBounce = isBounce
                });
            }
        }

        // Bulk insert in batches to keep memory/SQL statement size reasonable.
        const int batchSize = 2000;
        for (var offset = 0; offset < pageviews.Count; offset += batchSize)
        {
            var batch = pageviews.Skip(offset).Take(batchSize);
            db.Pageviews.AddRange(batch);
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
        }

        logger.LogInformation("Seed complete: {Articles} articles, {Pageviews} pageviews.", articles.Count, pageviews.Count);
    }
}
