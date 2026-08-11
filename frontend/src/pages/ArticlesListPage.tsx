import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { articlesApi } from "../api/articles";
import { extractErrorMessage } from "../api/client";
import { ErrorBanner, LoadingState, EmptyState } from "../components/UiHelpers";
import { formatDate } from "../utils/format";
import { useAuth } from "../context/AuthContext";
import { hasAtLeastRole } from "../utils/roles";
import type { ArticleListItem } from "../types";

export function ArticlesListPage() {
  const { user } = useAuth();
  const [search, setSearch] = useState("");
  const [category, setCategory] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);

  const [items, setItems] = useState<ArticleListItem[]>([]);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);

    articlesApi
      .search({ search: search || undefined, category: category || undefined, page, pageSize })
      .then((result) => {
        if (cancelled) return;
        setItems(result.items);
        setTotalPages(result.totalPages || 1);
      })
      .catch((err) => !cancelled && setError(extractErrorMessage(err)))
      .finally(() => !cancelled && setLoading(false));

    return () => {
      cancelled = true;
    };
  }, [search, category, page, pageSize]);

  const canEdit = hasAtLeastRole(user?.role, "Editor");

  return (
    <div className="page-content">
      <div className="page-header">
        <h1>Articles</h1>
        {canEdit && (
          <Link to="/articles/new" className="btn btn-primary">
            + New Article
          </Link>
        )}
      </div>

      <div className="filters-bar">
        <div className="form-group" style={{ marginBottom: 0 }}>
          <label>Search</label>
          <input
            placeholder="Search by title..."
            value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }}
          />
        </div>
        <div className="form-group" style={{ marginBottom: 0 }}>
          <label>Category</label>
          <select value={category} onChange={(e) => { setCategory(e.target.value); setPage(1); }}>
            <option value="">All categories</option>
            <option value="Technology">Technology</option>
            <option value="Business">Business</option>
            <option value="Lifestyle">Lifestyle</option>
          </select>
        </div>
      </div>

      {error && <ErrorBanner message={error} />}
      {loading && <LoadingState />}

      {!loading && !error && (
        <div className="card">
          {items.length === 0 ? (
            <EmptyState label="No articles found." />
          ) : (
            <>
              <table>
                <thead>
                  <tr>
                    <th>Title</th>
                    <th>Category</th>
                    <th>Tags</th>
                    <th>Authors</th>
                    <th>Published</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map((a) => (
                    <tr key={a.id}>
                      <td>
                        <Link to={`/articles/${a.id}`}>{a.title}</Link>
                      </td>
                      <td>{a.category}</td>
                      <td>
                        {a.tags.map((t) => (
                          <span key={t} className="tag-pill">
                            {t}
                          </span>
                        ))}
                      </td>
                      <td>{a.authors.join(", ")}</td>
                      <td>{formatDate(a.publishedAt)}</td>
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
