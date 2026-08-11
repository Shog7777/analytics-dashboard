import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { pageviewsApi } from "../api/pageviews";
import { articlesApi } from "../api/articles";
import { extractErrorMessage } from "../api/client";
import { ErrorBanner, LoadingState, EmptyState } from "../components/UiHelpers";
import { formatDateTime } from "../utils/format";
import type { ArticleListItem, Pageview } from "../types";

export function PageviewsPage() {
  const [searchParams] = useSearchParams();
  const [articleId, setArticleId] = useState<string>(searchParams.get("articleId") ?? "");
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize] = useState(20);

  const [articles, setArticles] = useState<ArticleListItem[]>([]);
  const [data, setData] = useState<Pageview[]>([]);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    articlesApi.search({ page: 1, pageSize: 100 }).then((r) => setArticles(r.items)).catch(() => {});
  }, []);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);

    pageviewsApi
      .get({
        articleId: articleId ? Number(articleId) : undefined,
        from: from || undefined,
        to: to ? `${to}T23:59:59` : undefined,
        page,
        pageSize,
      })
      .then((result) => {
        if (cancelled) return;
        setData(result.items);
        setTotalPages(result.totalPages || 1);
      })
      .catch((err) => !cancelled && setError(extractErrorMessage(err)))
      .finally(() => !cancelled && setLoading(false));

    return () => {
      cancelled = true;
    };
  }, [articleId, from, to, page, pageSize]);

  return (
    <div className="page-content">
      <div className="page-header">
        <h1>Pageviews</h1>
      </div>

      <div className="filters-bar">
        <div className="form-group" style={{ marginBottom: 0 }}>
          <label>Article</label>
          <select value={articleId} onChange={(e) => { setArticleId(e.target.value); setPage(1); }}>
            <option value="">All articles</option>
            {articles.map((a) => (
              <option key={a.id} value={a.id}>
                {a.title}
              </option>
            ))}
          </select>
        </div>
        <div className="form-group" style={{ marginBottom: 0 }}>
          <label>From</label>
          <input type="date" value={from} onChange={(e) => { setFrom(e.target.value); setPage(1); }} />
        </div>
        <div className="form-group" style={{ marginBottom: 0 }}>
          <label>To</label>
          <input type="date" value={to} onChange={(e) => { setTo(e.target.value); setPage(1); }} />
        </div>
      </div>

      {error && <ErrorBanner message={error} />}
      {loading && <LoadingState />}

      {!loading && !error && (
        <div className="card">
          {data.length === 0 ? (
            <EmptyState label="No pageviews match these filters." />
          ) : (
            <>
              <table>
                <thead>
                  <tr>
                    <th>Article</th>
                    <th>Viewed At</th>
                    <th>Duration</th>
                    <th>Bounce</th>
                  </tr>
                </thead>
                <tbody>
                  {data.map((p) => (
                    <tr key={p.id}>
                      <td>{p.articleTitle}</td>
                      <td>{formatDateTime(p.viewedAt)}</td>
                      <td>{p.durationSeconds}s</td>
                      <td>{p.isBounce ? "Yes" : "No"}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
              <div className="pagination">
                <button className="btn" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}>
                  Prev
                </button>
                <span>
                  Page {page} of {totalPages}
                </span>
                <button className="btn" disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)}>
                  Next
                </button>
              </div>
            </>
          )}
        </div>
      )}
    </div>
  );
}
