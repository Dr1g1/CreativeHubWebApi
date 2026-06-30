import { useEffect, useState } from "react";
import { getTopCreators, getRatingDistribution } from "../api/statistics";
import type { TopCreator, RatingBucket } from "../types";

export default function StatisticsPage() {
  const [creators, setCreators] = useState<TopCreator[]>([]);
  const [buckets, setBuckets] = useState<RatingBucket[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([getTopCreators(), getRatingDistribution()])
      .then(([c, b]) => {
        setCreators(c);
        setBuckets(b);
      })
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p style={{ padding: 20 }}>Učitavanje...</p>;

  const maxCount = Math.max(1, ...buckets.map((b) => b.count));

  const medal = (i: number) => `${i + 1}.`;

  return (
    <div style={{ padding: 20, maxWidth: 800, margin: "0 auto" }}>
      <h1>Statistika</h1>

      <h2 style={{ marginTop: 24 }}>Najaktivniji kreatori</h2>
      {creators.length === 0 ? (
        <p style={{ color: "#666" }}>Još nema podataka.</p>
      ) : (
        <div style={{ display: "grid", gap: 8 }}>
          {creators.map((c, i) => (
            <div key={c.creatorId}
              style={{ display: "flex", alignItems: "center", gap: 12,
                       padding: "10px 14px", border: "1px solid #eee", borderRadius: 8,
                       background: i < 3 ? "#f4f0f1" : "white" }}>
              <span style={{ fontSize: 20, width: 32 }}>{medal(i)}</span>
              <span style={{ flex: 1, fontWeight: 600 }}>{c.username}</span>
              <span style={{ fontSize: 14, color: "#666" }}>
                {c.resourceCount} resursa · ⬇ {c.totalDownloads}
              </span>
            </div>
          ))}
        </div>
      )}

      <h2 style={{ marginTop: 36 }}>Raspodela ocena</h2>
      {buckets.length === 0 ? (
        <p style={{ color: "#666" }}>Još nema ocenjenih resursa.</p>
      ) : (
        <div style={{ display: "grid", gap: 10, maxWidth: 520 }}>
          {buckets.map((b) => (
            <div key={b.range} style={{ display: "flex", alignItems: "center", gap: 12 }}>
              <span style={{ width: 60, fontSize: 14, color: "#666" }}>★ {b.range}</span>
              <div style={{ flex: 1, background: "var(--surface)", borderRadius: 6, height: 28,
                            position: "relative", overflow: "hidden" }}>
                <div style={{
                  width: `${(b.count / maxCount) * 100}%`,
                  height: "100%", background: "var(--burgundy)", borderRadius: 6,
                  transition: "width .3s",
                }} />
                <span style={{ position: "absolute", left: 10, top: 4, fontSize: 13,
                               fontWeight: 600, color: "var(--burgundy)" }}>
                  {b.count}
                </span>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
