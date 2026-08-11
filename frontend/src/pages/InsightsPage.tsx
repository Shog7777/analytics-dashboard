import { useEffect, useState } from "react";
import { analyticsApi } from "../api/analytics";
import { extractErrorMessage } from "../api/client";
import { DateRangeFilter, defaultDateRange, type DateRange } from "../components/DateRangeFilter";
import { ErrorBanner, LoadingState, EmptyState } from "../components/UiHelpers";
import { formatDuration, formatNumber } from "../utils/format";
import type { AuthorPerformance, CampaignImpact, TopTag } from "../types";

export function InsightsPage() {
  const [range, setRange] = useState<DateRange>(() => defaultDateRange(90));
  const [topTags, setTopTags] = useState<TopTag[]>([]);
  const [authorPerf, setAuthorPerf] = useState<AuthorPerformance[]>([]);
  const [campaignImpact, setCampaignImpact] = useState<CampaignImpact[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);

    const params = { from: range.from, to: `${range.to}T23:59:59` };

    Promise.all([
      analyticsApi.getTopTags({ ...params, limit: 10 }),
      analyticsApi.getAuthorPerformance(params),
      analyticsApi.getCampaignImpact(params),
    ])
      .then(([tags, authors, campaigns]) => {
        if (cancelled) return;
        setTopTags(tags);
        setAuthorPerf(authors);
        setCampaignImpact(campaigns);
      })
      .catch((err) => !cancelled && setError(extractErrorMessage(err)))
      .finally(() => !cancelled && setLoading(false));

    return () => {
      cancelled = true;
    };
  }, [range.from, range.to]);

  return (
    <div className="page-content">
      <div className="page-header">
        <h1>Insights</h1>
      </div>

      <DateRangeFilter value={range} onChange={setRange} />

      {error && <ErrorBanner message={error} />}
      {loading && <LoadingState />}

      {!loading && !error && (
        <div className="grid" style={{ gap: 20 }}>
          <div className="card">
            <h2 className="section-title">Top Tags</h2>
            {topTags.length === 0 ? (
              <EmptyState />
            ) : (
              <table>
                <thead>
                  <tr>
                    <th>Tag</th>
                    <th>Articles</th>
                    <th>Views</th>
                  </tr>
                </thead>
                <tbody>
                  {topTags.map((t) => (
                    <tr key={t.tagId}>
                      <td>{t.name}</td>
                      <td>{t.articleCount}</td>
                      <td>{formatNumber(t.views)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>

          <div className="card">
            <h2 className="section-title">Author Performance</h2>
            {authorPerf.length === 0 ? (
              <EmptyState />
            ) : (
              <table>
                <thead>
                  <tr>
                    <th>Author</th>
                    <th>Articles</th>
                    <th>Total Views</th>
                    <th>Avg. Time on Page</th>
                  </tr>
                </thead>
                <tbody>
                  {authorPerf.map((a) => (
                    <tr key={a.authorId}>
                      <td>{a.name}</td>
                      <td>{a.articleCount}</td>
                      <td>{formatNumber(a.totalViews)}</td>
                      <td>{formatDuration(a.avgTimeOnPageSeconds)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>

          <div className="card">
            <h2 className="section-title">Campaign Impact</h2>
            {campaignImpact.length === 0 ? (
              <EmptyState />
            ) : (
              <table>
                <thead>
                  <tr>
                    <th>Campaign</th>
                    <th>Articles</th>
                    <th>Total Views</th>
                    <th>Bounce Rate</th>
                  </tr>
                </thead>
                <tbody>
                  {campaignImpact.map((c) => (
                    <tr key={c.campaignId}>
                      <td>{c.name}</td>
                      <td>{c.articleCount}</td>
                      <td>{formatNumber(c.totalViews)}</td>
                      <td>{c.bounceRatePercent.toFixed(1)}%</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
