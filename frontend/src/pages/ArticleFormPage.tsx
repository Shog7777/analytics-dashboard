import { useEffect, useState, type FormEvent } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { articlesApi } from "../api/articles";
import { extractErrorMessage } from "../api/client";
import { ErrorBanner, LoadingState } from "../components/UiHelpers";
import { toDateInputValue } from "../utils/format";

export function ArticleFormPage() {
  const { id } = useParams();
  const isEdit = !!id;
  const navigate = useNavigate();

  const [title, setTitle] = useState("");
  const [category, setCategory] = useState("Technology");
  const [publishedAt, setPublishedAt] = useState(() => toDateInputValue(new Date()));
  const [loading, setLoading] = useState(isEdit);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!isEdit) return;
    articlesApi
      .getById(Number(id))
      .then((article) => {
        setTitle(article.title);
        setCategory(article.category);
        setPublishedAt(toDateInputValue(article.publishedAt));
      })
      .catch((err) => setError(extractErrorMessage(err)))
      .finally(() => setLoading(false));
  }, [id, isEdit]);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      const payload = { title, category, publishedAt: new Date(publishedAt).toISOString() };
      if (isEdit) {
        await articlesApi.update(Number(id), payload);
        navigate(`/articles/${id}`);
      } else {
        const created = await articlesApi.create(payload);
        navigate(`/articles/${created.id}`);
      }
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return (
      <div className="page-content">
        <LoadingState />
      </div>
    );
  }

  return (
    <div className="page-content" style={{ maxWidth: 560 }}>
      <div className="page-header">
        <h1>{isEdit ? "Edit Article" : "New Article"}</h1>
      </div>

      {error && <ErrorBanner message={error} />}

      <form className="card" onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Title</label>
          <input value={title} onChange={(e) => setTitle(e.target.value)} required minLength={3} />
        </div>

        <div className="form-group">
          <label>Category</label>
          <select value={category} onChange={(e) => setCategory(e.target.value)}>
            <option value="Technology">Technology</option>
            <option value="Business">Business</option>
            <option value="Lifestyle">Lifestyle</option>
          </select>
        </div>

        <div className="form-group">
          <label>Published date</label>
          <input type="date" value={publishedAt} onChange={(e) => setPublishedAt(e.target.value)} required />
        </div>

        <div style={{ display: "flex", gap: 10 }}>
          <button className="btn btn-primary" type="submit" disabled={saving}>
            {saving ? "Saving..." : "Save"}
          </button>
          <button className="btn" type="button" onClick={() => navigate(-1)}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
