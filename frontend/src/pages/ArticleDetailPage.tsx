import { useCallback, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { articlesApi } from "../api/articles";
import { authorsApi, campaignsApi, tagsApi } from "../api/lookups";
import { extractErrorMessage } from "../api/client";
import { ErrorBanner, LoadingState } from "../components/UiHelpers";
import { formatDate, formatDuration } from "../utils/format";
import { useAuth } from "../context/AuthContext";
import { hasAtLeastRole } from "../utils/roles";
import type { Article, Author, Campaign, Tag } from "../types";

export function ArticleDetailPage() {
  const { id } = useParams();
  const articleId = Number(id);
  const navigate = useNavigate();
  const { user } = useAuth();

  const [article, setArticle] = useState<Article | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);

  const [summary, setSummary] = useState("");
  const [heroImageUrl, setHeroImageUrl] = useState("");
  const [readingTimeSeconds, setReadingTimeSeconds] = useState(0);
  const [savingDetails, setSavingDetails] = useState(false);

  const [allTags, setAllTags] = useState<Tag[]>([]);
  const [allAuthors, setAllAuthors] = useState<Author[]>([]);
  const [allCampaigns, setAllCampaigns] = useState<Campaign[]>([]);
  const [selectedTagIds, setSelectedTagIds] = useState<number[]>([]);
  const [selectedAuthorIds, setSelectedAuthorIds] = useState<number[]>([]);
  const [selectedCampaignIds, setSelectedCampaignIds] = useState<number[]>([]);
  const [savingAssociations, setSavingAssociations] = useState(false);

  const canEdit = hasAtLeastRole(user?.role, "Editor");
  const canDelete = hasAtLeastRole(user?.role, "Admin");

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const [articleData, tagData, authorData, campaignData] = await Promise.all([
        articlesApi.getById(articleId),
        tagsApi.getAll(),
        authorsApi.getAll(),
        campaignsApi.getAll(),
      ]);
      setArticle(articleData);
      setAllTags(tagData);
      setAllAuthors(authorData);
      setAllCampaigns(campaignData);
      setSummary(articleData.details?.summary ?? "");
      setHeroImageUrl(articleData.details?.heroImageUrl ?? "");
      setReadingTimeSeconds(articleData.details?.readingTimeSeconds ?? 0);
      setSelectedTagIds(articleData.tags.map((t) => t.id));
      setSelectedAuthorIds(articleData.authors.map((a) => a.id));
      setSelectedCampaignIds(articleData.campaigns.map((c) => c.id));
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, [articleId]);

  useEffect(() => {
    load();
  }, [load]);

  async function handleSaveDetails() {
    setSavingDetails(true);
    setNotice(null);
    setError(null);
    try {
      await articlesApi.upsertDetails(articleId, {
        summary,
        heroImageUrl: heroImageUrl || undefined,
        readingTimeSeconds,
      });
      setNotice("Details saved.");
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setSavingDetails(false);
    }
  }

  async function handleSaveAssociations() {
    setSavingAssociations(true);
    setNotice(null);
    setError(null);
    try {
      await Promise.all([
        articlesApi.setTags(articleId, selectedTagIds),
        articlesApi.setAuthors(articleId, selectedAuthorIds),
        articlesApi.setCampaigns(articleId, selectedCampaignIds),
      ]);
      setNotice("Associations saved.");
      await load();
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setSavingAssociations(false);
    }
  }

  async function handleDelete() {
    if (!window.confirm("Delete this article? This also removes its pageviews and details.")) return;
    try {
      await articlesApi.remove(articleId);
      navigate("/articles");
    } catch (err) {
      setError(extractErrorMessage(err));
    }
  }

  function toggle(list: number[], setList: (v: number[]) => void, id: number) {
    setList(list.includes(id) ? list.filter((x) => x !== id) : [...list, id]);
  }

  if (loading) {
    return (
      <div className="page-content">
        <LoadingState />
      </div>
    );
  }

  if (error && !article) {
    return (
      <div className="page-content">
        <ErrorBanner message={error} />
      </div>
    );
  }

  if (!article) return null;

  return (
    <div className="page-content">
      <div className="page-header">
        <div>
          <h1>{article.title}</h1>
          <p style={{ color: "var(--color-text-muted)", margin: "4px 0 0", fontSize: "0.85rem" }}>
            {article.category} · Published {formatDate(article.publishedAt)}
          </p>
        </div>
        {canEdit && (
          <div style={{ display: "flex", gap: 8 }}>
            <Link to={`/articles/${article.id}/edit`} className="btn">
              Edit
            </Link>
            <Link to={`/pageviews?articleId=${article.id}`} className="btn">
              View Pageviews
            </Link>
            {canDelete && (
              <button className="btn btn-danger" onClick={handleDelete}>
                Delete
              </button>
            )}
          </div>
        )}
      </div>

      {notice && <div className="card" style={{ marginBottom: 16, color: "var(--color-success)" }}>{notice}</div>}
      {error && <ErrorBanner message={error} />}

      <div className="grid grid-cols-2" style={{ alignItems: "start" }}>
        <div className="card">
          <h2 className="section-title">Details</h2>
          <div className="form-group">
            <label>Summary</label>
            <textarea
              rows={4}
              value={summary}
              onChange={(e) => setSummary(e.target.value)}
              disabled={!canEdit}
            />
          </div>
          <div className="form-group">
            <label>Hero image URL</label>
            <input value={heroImageUrl} onChange={(e) => setHeroImageUrl(e.target.value)} disabled={!canEdit} />
          </div>
          <div className="form-group">
            <label>Reading time: {formatDuration(readingTimeSeconds)}</label>
            <input
              type="number"
              min={0}
              value={readingTimeSeconds}
              onChange={(e) => setReadingTimeSeconds(Number(e.target.value))}
              disabled={!canEdit}
            />
          </div>
          {canEdit && (
            <button className="btn btn-primary" onClick={handleSaveDetails} disabled={savingDetails}>
              {savingDetails ? "Saving..." : "Save Details"}
            </button>
          )}
        </div>

        <div className="card">
          <h2 className="section-title">Tags</h2>
          <div className="chip-row" style={{ marginBottom: 16 }}>
            {allTags.map((tag) => (
              <button
                key={tag.id}
                type="button"
                className={`chip ${selectedTagIds.includes(tag.id) ? "selected" : ""}`}
                onClick={() => canEdit && toggle(selectedTagIds, setSelectedTagIds, tag.id)}
                disabled={!canEdit}
              >
                {tag.name}
              </button>
            ))}
          </div>

          <h2 className="section-title">Authors</h2>
          <div className="chip-row" style={{ marginBottom: 16 }}>
            {allAuthors.map((author) => (
              <button
                key={author.id}
                type="button"
                className={`chip ${selectedAuthorIds.includes(author.id) ? "selected" : ""}`}
                onClick={() => canEdit && toggle(selectedAuthorIds, setSelectedAuthorIds, author.id)}
                disabled={!canEdit}
              >
                {author.name}
              </button>
            ))}
          </div>

          <h2 className="section-title">Campaigns</h2>
          <div className="chip-row" style={{ marginBottom: 16 }}>
            {allCampaigns.map((campaign) => (
              <button
                key={campaign.id}
                type="button"
                className={`chip ${selectedCampaignIds.includes(campaign.id) ? "selected" : ""}`}
                onClick={() => canEdit && toggle(selectedCampaignIds, setSelectedCampaignIds, campaign.id)}
                disabled={!canEdit}
              >
                {campaign.name}
              </button>
            ))}
          </div>

          {canEdit && (
            <button className="btn btn-primary" onClick={handleSaveAssociations} disabled={savingAssociations}>
              {savingAssociations ? "Saving..." : "Save Associations"}
            </button>
          )}
        </div>
      </div>
    </div>
  );
}
