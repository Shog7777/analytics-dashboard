import { apiClient } from "./client";
import type {
  AuthorPerformance,
  CampaignImpact,
  DailyViewsPoint,
  KpiSummary,
  TopArticle,
  TopTag,
} from "../types";

export interface DateRangeParams {
  from?: string;
  to?: string;
}

export const analyticsApi = {
  getKpis: (params: DateRangeParams) =>
    apiClient.get<KpiSummary>("/analytics/kpis", { params }).then((r) => r.data),

  getDailyViews: (params: DateRangeParams) =>
    apiClient.get<DailyViewsPoint[]>("/analytics/daily-views", { params }).then((r) => r.data),

  getTopArticles: (params: DateRangeParams & { limit?: number }) =>
    apiClient.get<TopArticle[]>("/analytics/top-articles", { params }).then((r) => r.data),

  getTopTags: (params: DateRangeParams & { limit?: number }) =>
    apiClient.get<TopTag[]>("/analytics/top-tags", { params }).then((r) => r.data),

  getAuthorPerformance: (params: DateRangeParams) =>
    apiClient.get<AuthorPerformance[]>("/analytics/author-performance", { params }).then((r) => r.data),

  getCampaignImpact: (params: DateRangeParams) =>
    apiClient.get<CampaignImpact[]>("/analytics/campaign-impact", { params }).then((r) => r.data),
};
