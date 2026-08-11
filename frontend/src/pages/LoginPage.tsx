import { useState, type FormEvent } from "react";
import { Navigate, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { extractErrorMessage } from "../api/client";
import { ErrorBanner } from "../components/UiHelpers";

export function LoginPage() {
  const { login, register, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [mode, setMode] = useState<"login" | "register">("login");
  const [usernameOrEmail, setUsernameOrEmail] = useState("");
  const [password, setPassword] = useState("");
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  if (isAuthenticated) {
    const redirectTo = (location.state as { from?: string } | null)?.from ?? "/";
    return <Navigate to={redirectTo} replace />;
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      if (mode === "login") {
        await login(usernameOrEmail, password);
      } else {
        await register(username, email, password);
      }
      navigate("/", { replace: true });
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="login-shell">
      <div className="card login-card">
        <h1>{mode === "login" ? "Sign in" : "Create account"}</h1>
        <p style={{ color: "var(--color-text-muted)", fontSize: "0.85rem", marginTop: -8 }}>
          Analytics Dashboard
        </p>

        {error && <ErrorBanner message={error} />}

        <form onSubmit={handleSubmit}>
          {mode === "login" ? (
            <div className="form-group">
              <label>Username or email</label>
              <input
                value={usernameOrEmail}
                onChange={(e) => setUsernameOrEmail(e.target.value)}
                required
                autoFocus
              />
            </div>
          ) : (
            <>
              <div className="form-group">
                <label>Username</label>
                <input value={username} onChange={(e) => setUsername(e.target.value)} required minLength={3} />
              </div>
              <div className="form-group">
                <label>Email</label>
                <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
              </div>
            </>
          )}

          <div className="form-group">
            <label>Password</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              minLength={6}
            />
          </div>

          <button className="btn btn-primary" type="submit" disabled={submitting} style={{ width: "100%" }}>
            {submitting ? "Please wait..." : mode === "login" ? "Sign in" : "Create account"}
          </button>
        </form>

        <p style={{ fontSize: "0.85rem", marginTop: 14, color: "var(--color-text-muted)" }}>
          {mode === "login" ? (
            <>
              New here?{" "}
              <a href="#" onClick={(e) => { e.preventDefault(); setMode("register"); setError(null); }}>
                Create an account
              </a>
            </>
          ) : (
            <>
              Already have an account?{" "}
              <a href="#" onClick={(e) => { e.preventDefault(); setMode("login"); setError(null); }}>
                Sign in
              </a>
            </>
          )}
        </p>

        <p style={{ fontSize: "0.78rem", marginTop: 18, color: "var(--color-text-muted)" }}>
          Demo accounts (seeded): <br />
          admin / Admin@123 &nbsp;·&nbsp; editor / Editor@123 &nbsp;·&nbsp; viewer / Viewer@123
        </p>
      </div>
    </div>
  );
}
