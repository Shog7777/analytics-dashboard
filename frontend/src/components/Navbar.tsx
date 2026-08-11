import { NavLink } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { hasAtLeastRole } from "../utils/roles";

export function Navbar() {
  const { user, logout } = useAuth();

  if (!user) return null;

  return (
    <header className="navbar">
      <div style={{ display: "flex", alignItems: "center", gap: 28 }}>
        <NavLink to="/" className="navbar-brand">
          Analytics Dashboard
        </NavLink>
        <nav className="navbar-links">
          <NavLink to="/" end>
            Dashboard
          </NavLink>
          <NavLink to="/articles">Articles</NavLink>
          <NavLink to="/pageviews">Pageviews</NavLink>
          <NavLink to="/insights">Insights</NavLink>
          {hasAtLeastRole(user.role, "Editor") && <NavLink to="/manage">Manage</NavLink>}
        </nav>
      </div>
      <div className="navbar-user">
        <span>{user.username}</span>
        <span className="role-badge">{user.role}</span>
        <button className="btn" onClick={logout}>
          Log out
        </button>
      </div>
    </header>
  );
}
