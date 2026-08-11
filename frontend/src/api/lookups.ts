import { apiClient } from "./client";
import type { Author, Campaign, Tag } from "../types";

export const tagsApi = {
  getAll: () => apiClient.get<Tag[]>("/tags").then((r) => r.data),
  create: (name: string) => apiClient.post<Tag>("/tags", { name }).then((r) => r.data),
  remove: (id: number) => apiClient.delete(`/tags/${id}`),
};

export const authorsApi = {
  getAll: () => apiClient.get<Author[]>("/authors").then((r) => r.data),
  create: (payload: { name: string; email: string; bio?: string }) =>
    apiClient.post<Author>("/authors", payload).then((r) => r.data),
  remove: (id: number) => apiClient.delete(`/authors/${id}`),
};

export const campaignsApi = {
  getAll: () => apiClient.get<Campaign[]>("/campaigns").then((r) => r.data),
  create: (payload: { name: string; description?: string; startDate: string; endDate?: string }) =>
    apiClient.post<Campaign>("/campaigns", payload).then((r) => r.data),
  remove: (id: number) => apiClient.delete(`/campaigns/${id}`),
};
