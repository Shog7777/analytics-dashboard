import { useEffect, useState, type FormEvent } from "react";
import { authorsApi, campaignsApi, tagsApi } from "../api/lookups";
import { extractErrorMessage } from "../api/client";
import { ErrorBanner, LoadingState } from "../components/UiHelpers";
import { useAuth } from "../context/AuthContext";
import { hasAtLeastRole } from "../utils/roles";
import { UsersPanel } from "../components/UsersPanel";
import type { Author, Campaign, Tag } from "../types";

export function ManagePage() {
  const { user } = useAuth();
  const [tags, setTags] = useState<Tag[]>([]);
  const [authors, setAuthors] = useState<Author[]>([]);
  const [campaigns, setCampaigns] = useState<Campaign[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [newTag, setNewTag] = useState("");
  const [newAuthorName, setNewAuthorName] = useState("");
  const [newAuthorEmail, setNewAuthorEmail] = useState("");
  const [newCampaignName, setNewCampaignName] = useState("");
  const [newCampaignStart, setNewCampaignStart] = useState(() => new Date().toISOString().slice(0, 10));

  const canDelete = hasAtLeastRole(user?.role, "Admin");
  const isAdmin = hasAtLeastRole(user?.role, "Admin");

  async function loadAll() {
    setLoading(true);
    setError(null);
    try {
      const [tagData, authorData, campaignData] = await Promise.all([
        tagsApi.getAll(),
        authorsApi.getAll(),
        campaignsApi.getAll(),
      ]);
      setTags(tagData);
      setAuthors(authorData);
      setCampaigns(campaignData);
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadAll();
  }, []);

  async function handleAddTag(e: FormEvent) {
    e.preventDefault();
    if (!newTag.trim()) return;
    try {
      await tagsApi.create(newTag.trim());
      setNewTag("");
      loadAll();
    } catch (err) {
      setError(extractErrorMessage(err));
    }
  }

  async function handleAddAuthor(e: FormEvent) {
    e.preventDefault();
    if (!newAuthorName.trim() || !newAuthorEmail.trim()) return;
    try {
      await authorsApi.create({ name: newAuthorName.trim(), email: newAuthorEmail.trim() });
      setNewAuthorName("");
      setNewAuthorEmail("");
      loadAll();
    } catch (err) {
      setError(extractErrorMessage(err));
    }
  }

  async function handleAddCampaign(e: FormEvent) {
    e.preventDefault();
    if (!newCampaignName.trim()) return;
    try {
      await campaignsApi.create({ name: newCampaignName.trim(), startDate: new Date(newCampaignStart).toISOString() });
      setNewCampaignName("");
      loadAll();
    } catch (err) {
      setError(extractErrorMessage(err));
    }
  }

  async function handleDelete(kind: "tag" | "author" | "campaign", id: number) {
    if (!window.confirm("Delete this item?")) return;
    try {
      if (kind === "tag") await tagsApi.remove(id);
      if (kind === "author") await authorsApi.remove(id);
      if (kind === "campaign") await campaignsApi.remove(id);
      loadAll();
    } catch (err) {
      setError(extractErrorMessage(err));
    }
  }

  return (
    <div className="page-content">
      <div className="page-header">
        <h1>Manage</h1>
      </div>

      {error && <ErrorBanner message={error} />}
      {loading ? (
        <LoadingState />
      ) : (
        <div className="grid grid-cols-3" style={{ alignItems: "start" }}>
          <div className="card">
            <h2 className="section-title">Tags</h2>
            <form onSubmit={handleAddTag} style={{ display: "flex", gap: 8, marginBottom: 14 }}>
              <input placeholder="New tag" value={newTag} onChange={(e) => setNewTag(e.target.value)} />
              <button className="btn btn-primary" type="submit">
                Add
              </button>
            </form>
            <div className="chip-row">
              {tags.map((t) => (
                <span key={t.id} className="chip" style={{ display: "flex", alignItems: "center", gap: 6 }}>
                  {t.name}
                  {canDelete && (
                    <button
                      onClick={() => handleDelete("tag", t.id)}
                      style={{ background: "none", border: "none", color: "var(--color-danger)", cursor: "pointer" }}
                    >
                      ×
                    </button>
                  )}
                </span>
              ))}
            </div>
          </div>

          <div className="card">
            <h2 className="section-title">Authors</h2>
            <form onSubmit={handleAddAuthor} style={{ display: "flex", flexDirection: "column", gap: 8, marginBottom: 14 }}>
              <input placeholder="Name" value={newAuthorName} onChange={(e) => setNewAuthorName(e.target.value)} />
              <input
                placeholder="Email"
                type="email"
                value={newAuthorEmail}
                onChange={(e) => setNewAuthorEmail(e.target.value)}
              />
              <button className="btn btn-primary" type="submit">
                Add Author
              </button>
            </form>
            <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
              {authors.map((a) => (
                <li
                  key={a.id}
                  style={{ display: "flex", justifyContent: "space-between", padding: "6px 0", borderBottom: "1px solid var(--color-border)" }}
                >
                  <span>{a.name}</span>
                  {canDelete && (
                    <button
                      onClick={() => handleDelete("author", a.id)}
                      style={{ background: "none", border: "none", color: "var(--color-danger)", cursor: "pointer" }}
                    >
                      Remove
                    </button>
                  )}
                </li>
              ))}
            </ul>
          </div>

          <div className="card">
            <h2 className="section-title">Campaigns</h2>
            <form onSubmit={handleAddCampaign} style={{ display: "flex", flexDirection: "column", gap: 8, marginBottom: 14 }}>
              <input placeholder="Name" value={newCampaignName} onChange={(e) => setNewCampaignName(e.target.value)} />
              <input type="date" value={newCampaignStart} onChange={(e) => setNewCampaignStart(e.target.value)} />
              <button className="btn btn-primary" type="submit">
                Add Campaign
              </button>
            </form>
            <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
              {campaigns.map((c) => (
                <li
                  key={c.id}
                  style={{ display: "flex", justifyContent: "space-between", padding: "6px 0", borderBottom: "1px solid var(--color-border)" }}
                >
                  <span>{c.name}</span>
                  {canDelete && (
                    <button
                      onClick={() => handleDelete("campaign", c.id)}
                      style={{ background: "none", border: "none", color: "var(--color-danger)", cursor: "pointer" }}
                    >
                      Remove
                    </button>
                  )}
                </li>
              ))}
            </ul>
          </div>
        </div>
      )}

      {isAdmin && (
        <div style={{ marginTop: 20 }}>
          <UsersPanel />
        </div>
      )}
    </div>
  );
}
