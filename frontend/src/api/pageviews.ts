import { apiClient } from "./client";
import type { PagedResult, Pageview } from "../types";

export interface PageviewFilters {
  articleId?: number;
  from?: string;
  to?: string;
  page?: number;
  pageSize?: number;
}

export const pageviewsApi = {
  get: (params: PageviewFilters) =>
    apiClient.get<PagedResult<Pageview>>("/pageviews", { params }).then((r) => r.data),

  remove: (id: number) => apiClient.delete(`/pageviews/${id}`),
};
