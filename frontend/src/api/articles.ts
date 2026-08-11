import { apiClient } from "./client";
import type { Article, ArticleDetails, ArticleListItem, PagedResult } from "../types";

export interface ArticleSearchParams {
  search?: string;
  category?: string;
  tagId?: number;
  authorId?: number;
  page?: number;
  pageSize?: number;
}

export const articlesApi = {
  search: (params: ArticleSearchParams) =>
    apiClient.get<PagedResult<ArticleListItem>>("/articles", { params }).then((r) => r.data),

  getById: (id: number) => apiClient.get<Article>(`/articles/${id}`).then((r) => r.data),

  create: (payload: { title: string; category: string; publishedAt: string }) =>
    apiClient.post<Article>("/articles", payload).then((r) => r.data),

  update: (id: number, payload: { title: string; category: string; publishedAt: string }) =>
    apiClient.put<Article>(`/articles/${id}`, payload).then((r) => r.data),

  remove: (id: number) => apiClient.delete(`/articles/${id}`),

  upsertDetails: (id: number, payload: { summary: string; heroImageUrl?: string; readingTimeSeconds: number }) =>
    apiClient.put<ArticleDetails>(`/articles/${id}/details`, payload).then((r) => r.data),

  setTags: (id: number, ids: number[]) => apiClient.put(`/articles/${id}/tags`, { ids }).then((r) => r.data),

  setAuthors: (id: number, ids: number[]) => apiClient.put(`/articles/${id}/authors`, { ids }).then((r) => r.data),

  setCampaigns: (id: number, ids: number[]) => apiClient.put(`/articles/${id}/campaigns`, { ids }).then((r) => r.data),
};
