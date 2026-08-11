import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";
import { authApi } from "../api/auth";
import { getStoredToken, setStoredToken } from "../api/client";
import type { User } from "../types";

const USER_STORAGE_KEY = "analytics_dashboard_user";

interface AuthContextValue {
  user: User | null;
  isAuthenticated: boolean;
  login: (usernameOrEmail: string, password: string) => Promise<void>;
  register: (username: string, email: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function loadStoredUser(): User | null {
  const raw = localStorage.getItem(USER_STORAGE_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as User;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(() => (getStoredToken() ? loadStoredUser() : null));

  const persist = useCallback((token: string, nextUser: User) => {
    setStoredToken(token);
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(nextUser));
    setUser(nextUser);
  }, []);

  const login = useCallback(
    async (usernameOrEmail: string, password: string) => {
      const result = await authApi.login(usernameOrEmail, password);
      persist(result.token, result.user);
    },
    [persist]
  );

  const register = useCallback(
    async (username: string, email: string, password: string) => {
      const result = await authApi.register(username, email, password);
      persist(result.token, result.user);
    },
    [persist]
  );

  const logout = useCallback(() => {
    setStoredToken(null);
    localStorage.removeItem(USER_STORAGE_KEY);
    setUser(null);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({ user, isAuthenticated: !!user, login, register, logout }),
    [user, login, register, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within an AuthProvider");
  return ctx;
}
