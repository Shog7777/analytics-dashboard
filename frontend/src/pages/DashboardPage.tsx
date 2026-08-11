import { useEffect, useState } from "react";
import {
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import { analyticsApi } from "../api/analytics";
import { pageviewsApi } from "../api/pageviews";
import { extractErrorMessage } from "../api/client";
import { DateRangeFilter, defaultDateRange, type DateRange } from "../components/DateRangeFilter";
import { ErrorBanner, LoadingState, EmptyState } from "../components/UiHelpers";
import { formatDate, formatDateTime, formatDuration, formatNumber } from "../utils/format";
import type { DailyViewsPoint, KpiSummary, Pageview, TopArticle } from "../types";

export function DashboardPage() {
  const [range, setRange] = useState<DateRange>(() => defaultDateRange(30));
  const [kpis, setKpis] = useState<KpiSummary | null>(null);
  const [dailyViews, setDailyViews] = useState<DailyViewsPoint[]>([]);
  const [topArticles, setTopArticles] = useState<TopArticle[]>([]);
  const [recentPageviews, setRecentPageviews] = useState<Pageview[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);

    const params = { from: range.from, to: `${range.to}T23:59:59` };

    Promise.all([
      analyticsApi.getKpis(params),
      analyticsApi.getDailyViews(params),
      analyticsApi.getTopArticles({ ...params, limit: 5 }),
      pageviewsApi.get({ from: params.from, to: params.to, page: 1, pageSize: 10 }),
    ])
      .then(([kpiData, dailyData, topData, pageviewData]) => {
        if (cancelled) return;
        setKpis(kpiData);
        setDailyViews(dailyData);
        setTopArticles(topData);
        setRecentPageviews(pageviewData.items);
      })
      .catch((err) => !cancelled && setError(extractErrorMessage(err)))
      .finally(() => !cancelled && setLoading(false));

    return () => {
      cancelled = true;
    };
  }, [range.from, range.to]);

  const chartData = dailyViews.map((d) => ({ date: formatDate(d.date), views: d.views }));

  return (
    <div className="page-content">
      <div className="page-header">
        <h1>Dashboard</h1>
      </div>

      <DateRangeFilter value={range} onChange={setRange} />

      {error && <ErrorBanner message={error} />}
      {loading && <LoadingState />}

      {!loading && !error && (
        <>
          <div className="grid grid-cols-3" style={{ marginBottom: 20 }}>
            <div className="card kpi-card">
              <div className="kpi-label">Total Views</div>
              <div className="kpi-value">{formatNumber(kpis?.totalViews ?? 0)}</div>
            </div>
            <div className="card kpi-card">
              <div className="kpi-label">Avg. Time on Page</div>
              <div className="kpi-value">{formatDuration(kpis?.avgTimeOnPageSeconds ?? 0)}</div>
            </div>
            <div className="card kpi-card">
              <div className="kpi-label">Bounce Rate</div>
              <div className="kpi-value">{(kpis?.bounceRatePercent ?? 0).toFixed(1)}%</div>
            </div>
          </div>

          <div className="card" style={{ marginBottom: 20 }}>
            <h2 className="section-title">Daily Views</h2>
            {chartData.length === 0 ? (
              <EmptyState label="No pageview data in this range." />
            ) : (
              <ResponsiveContainer width="100%" height={280}>
                <LineChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#2b3348" />
                  <XAxis dataKey="date" stroke="#9aa4bd" fontSize={12} />
                  <YAxis stroke="#9aa4bd" fontSize={12} />
                  <Tooltip
                    contentStyle={{ background: "#1f2637", border: "1px solid #2b3348", borderRadius: 8 }}
                  />
                  <Line type="monotone" dataKey="views" stroke="#6c8cff" strokeWidth={2} dot={false} />
                </LineChart>
              </ResponsiveContainer>
            )}
          </div>

          <div className="grid grid-cols-2">
            <div className="card">
              <h2 className="section-title">Top Articles</h2>
              {topArticles.length === 0 ? (
                <EmptyState />
              ) : (
                <table>
                  <thead>
                    <tr>
                      <th>Title</th>
                      <th>Views</th>
                      <th>Avg. Time</th>
                    </tr>
                  </thead>
                  <tbody>
                    {topArticles.map((a) => (
                      <tr key={a.articleId}>
                        <td>{a.title}</td>
                        <td>{formatNumber(a.views)}</td>
                        <td>{formatDuration(a.avgTimeOnPageSeconds)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </div>

            <div className="card">
              <h2 className="section-title">Recent Pageviews</h2>
              {recentPageviews.length === 0 ? (
                <EmptyState />
              ) : (
                <table>
                  <thead>
                    <tr>
                      <th>Article</th>
                      <th>Viewed At</th>
                      <th>Bounce</th>
                    </tr>
                  </thead>
                  <tbody>
                    {recentPageviews.map((p) => (
                      <tr key={p.id}>
                        <td>{p.articleTitle}</td>
                        <td>{formatDateTime(p.viewedAt)}</td>
                        <td>{p.isBounce ? "Yes" : "No"}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </div>
          </div>
        </>
      )}
    </div>
  );
}
