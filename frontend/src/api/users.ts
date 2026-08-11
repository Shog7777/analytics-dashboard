import { apiClient } from "./client";
import type { User } from "../types";

export const usersApi = {
  getAll: () => apiClient.get<User[]>("/users").then((r) => r.data),
  updateRole: (id: number, role: string) => apiClient.put<User>(`/users/${id}/role`, { role }).then((r) => r.data),
};
