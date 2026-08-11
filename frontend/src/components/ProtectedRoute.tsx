import type { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import type { Role } from "../types";
import { hasAtLeastRole } from "../utils/roles";

interface ProtectedRouteProps {
  children: ReactNode;
  minimumRole?: Role;
}

export function ProtectedRoute({ children, minimumRole = "Viewer" }: ProtectedRouteProps) {
  const { isAuthenticated, user } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (!hasAtLeastRole(user?.role, minimumRole)) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}
