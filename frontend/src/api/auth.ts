import { apiClient } from "./client";
import type { AuthResponse } from "../types";

export const authApi = {
  login: (usernameOrEmail: string, password: string) =>
    apiClient.post<AuthResponse>("/auth/login", { usernameOrEmail, password }).then((r) => r.data),

  register: (username: string, email: string, password: string) =>
    apiClient.post<AuthResponse>("/auth/register", { username, email, password }).then((r) => r.data),
};
