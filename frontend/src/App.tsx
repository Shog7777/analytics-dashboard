import { Navigate, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import { Navbar } from "./components/Navbar";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { LoginPage } from "./pages/LoginPage";
import { DashboardPage } from "./pages/DashboardPage";
import { InsightsPage } from "./pages/InsightsPage";
import { PageviewsPage } from "./pages/PageviewsPage";
import { ArticlesListPage } from "./pages/ArticlesListPage";
import { ArticleDetailPage } from "./pages/ArticleDetailPage";
import { ArticleFormPage } from "./pages/ArticleFormPage";
import { ManagePage } from "./pages/ManagePage";

export default function App() {
  return (
    <AuthProvider>
      <div className="app-shell">
        <Navbar />
        <Routes>
          <Route path="/login" element={<LoginPage />} />

          <Route
            path="/"
            element={
              <ProtectedRoute>
                <DashboardPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/insights"
            element={
              <ProtectedRoute>
                <InsightsPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/pageviews"
            element={
              <ProtectedRoute>
                <PageviewsPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/articles"
            element={
              <ProtectedRoute>
                <ArticlesListPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/articles/new"
            element={
              <ProtectedRoute minimumRole="Editor">
                <ArticleFormPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/articles/:id/edit"
            element={
              <ProtectedRoute minimumRole="Editor">
                <ArticleFormPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/articles/:id"
            element={
              <ProtectedRoute>
                <ArticleDetailPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/manage"
            element={
              <ProtectedRoute minimumRole="Editor">
                <ManagePage />
              </ProtectedRoute>
            }
          />

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </div>
    </AuthProvider>
  );
}
