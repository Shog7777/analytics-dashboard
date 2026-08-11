import { useEffect, useState } from "react";
import { usersApi } from "../api/users";
import { extractErrorMessage } from "../api/client";
import { ErrorBanner, LoadingState } from "./UiHelpers";
import { useAuth } from "../context/AuthContext";
import type { Role, User } from "../types";

const ROLES: Role[] = ["Viewer", "Editor", "Admin"];

export function UsersPanel() {
  const { user: currentUser } = useAuth();
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [savingId, setSavingId] = useState<number | null>(null);

  function load() {
    setLoading(true);
    setError(null);
    usersApi
      .getAll()
      .then(setUsers)
      .catch((err) => setError(extractErrorMessage(err)))
      .finally(() => setLoading(false));
  }

  useEffect(load, []);

  async function handleRoleChange(id: number, role: string) {
    setSavingId(id);
    setError(null);
    try {
      const updated = await usersApi.updateRole(id, role);
      setUsers((prev) => prev.map((u) => (u.id === id ? updated : u)));
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setSavingId(null);
    }
  }

  return (
    <div className="card">
      <h2 className="section-title">Users &amp; Roles (Admin)</h2>
      {error && <ErrorBanner message={error} />}
      {loading ? (
        <LoadingState />
      ) : (
        <table>
          <thead>
            <tr>
              <th>Username</th>
              <th>Email</th>
              <th>Role</th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.id}>
                <td>{u.username}</td>
                <td>{u.email}</td>
                <td>
                  <select
                    value={u.role}
                    disabled={u.id === currentUser?.id || savingId === u.id}
                    onChange={(e) => handleRoleChange(u.id, e.target.value)}
                  >
                    {ROLES.map((r) => (
                      <option key={r} value={r}>
                        {r}
                      </option>
                    ))}
                  </select>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
