export type Role = "Viewer" | "Editor" | "Admin";

export interface User {
  id: number;
  username: string;
  email: string;
  role: Role;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: User;
}

export interface TagRef {
  id: number;
  name: string;
}

export interface AuthorRef {
  id: number;
  name: string;
}

export interface CampaignRef {
  id: number;
  name: string;
}

export interface ArticleListItem {
  id: number;
  title: string;
  category: string;
  publishedAt: string;
  tags: string[];
  authors: string[];
}

export interface ArticleDetails {
  articleId: number;
  summary: string;
  heroImageUrl?: string | null;
  readingTimeSeconds: number;
}

export interface Article {
  id: number;
  title: string;
  category: string;
  publishedAt: string;
  createdAt: string;
  updatedAt?: string | null;
  details?: ArticleDetails | null;
  tags: TagRef[];
  authors: AuthorRef[];
  campaigns: CampaignRef[];
}

export interface Pageview {
  id: number;
  articleId: number;
  articleTitle: string;
  viewedAt: string;
  durationSeconds: number;
  isBounce: boolean;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface KpiSummary {
  totalViews: number;
  avgTimeOnPageSeconds: number;
  bounceRatePercent: number;
  periodStart: string;
  periodEnd: string;
}

export interface DailyViewsPoint {
  date: string;
  views: number;
}

export interface TopArticle {
  articleId: number;
  title: string;
  category: string;
  views: number;
  avgTimeOnPageSeconds: number;
}

export interface TopTag {
  tagId: number;
  name: string;
  views: number;
  articleCount: number;
}

export interface AuthorPerformance {
  authorId: number;
  name: string;
  articleCount: number;
  totalViews: number;
  avgTimeOnPageSeconds: number;
}

export interface CampaignImpact {
  campaignId: number;
  name: string;
  articleCount: number;
  totalViews: number;
  bounceRatePercent: number;
}

export interface Author {
  id: number;
  name: string;
  email: string;
  bio?: string | null;
}

export interface Campaign {
  id: number;
  name: string;
  description?: string | null;
  startDate: string;
  endDate?: string | null;
}

export interface Tag {
  id: number;
  name: string;
}
