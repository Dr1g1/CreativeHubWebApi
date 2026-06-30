import { useEffect, useState } from "react";
import { searchResources, getFacets } from "../api/search";
import type { Resource, FacetResult } from "../types";
import ResourceCard from "../components/ResourceCard";
import { useAuth } from "../context/AuthContext";

export default function HomePage() {
  const [resources, setResources] = useState<Resource[]>([]);
  const [facets, setFacets] = useState<FacetResult | null>(null);
  const [loading, setLoading] = useState(true);

  const { userId } = useAuth();

  // kriterijumi pretrage
  const [text, setText] = useState("");
  const [type, setType] = useState("");
  const [selectedTags, setSelectedTags] = useState<string[]>([]);
  const [sortBy, setSortBy] = useState("newest");

  useEffect(() => {
    getFacets().then(setFacets).catch(() => {});
  }, []);

  useEffect(() => {
    const timer = setTimeout(() => {
      setLoading(true);
      searchResources({
        text: text || undefined,
        type: type || undefined,
        tags: selectedTags.length > 0 ? selectedTags : undefined,
        sortBy,
        excludeOwnerId: userId || undefined,  
      })
        .then((result) => setResources(result.items))
        .catch(() => setResources([]))
        .finally(() => setLoading(false));
    }, 300);

    return () => clearTimeout(timer);
  }, [text, type, selectedTags, sortBy, userId]);

  function toggleTag(tag: string) {
    setSelectedTags((prev) =>
      prev.includes(tag) ? prev.filter((t) => t !== tag) : [...prev, tag]
    );
  }

  return (
    <div style={{ padding: 20 }}>
      <h1>Resursi</h1>

      
      <div style={{ display: "flex", gap: 8, marginBottom: 16, flexWrap: "wrap" }}>
        <input
          placeholder="Pretraži po naslovu/opisu..."
          value={text}
          onChange={(e) => setText(e.target.value)}
          style={{ flex: 1, minWidth: 200, padding: 8, borderRadius: 8, border: "1px solid #ccc" }}
        />
        <select
          value={sortBy}
          onChange={(e) => setSortBy(e.target.value)}
          style={{ padding: 8, borderRadius: 8, border: "1px solid #ccc" }}
        >
          <option value="newest">Najnovije</option>
          <option value="downloads">Najskidanije</option>
          <option value="rating">Najbolje ocenjeno</option>
        </select>
      </div>

      {facets && (
        <div style={{ display: "flex", gap: 8, marginBottom: 12, flexWrap: "wrap" }}>
          <button
            onClick={() => setType("")}
            style={{
              fontWeight: type === "" ? "bold" : "normal",
              borderRadius: 8, padding: "6px 12px", cursor: "pointer",
            }}
          >
            Sve
          </button>
          {facets.byType.map((f) => (
            <button
              key={f.value}
              onClick={() => setType(f.value)}
              style={{
                fontWeight: type === f.value ? "bold" : "normal",
                borderRadius: 8, padding: "6px 12px", cursor: "pointer",
              }}
            >
              {f.value} ({f.count})
            </button>
          ))}
        </div>
      )}

      {facets && facets.topTags.length > 0 && (
        <div style={{ display: "flex", gap: 6, marginBottom: 16, flexWrap: "wrap" }}>
          {facets.topTags.map((t) => (
            <button
              key={t.value}
              onClick={() => toggleTag(t.value)}
              style={{
                fontSize: 13, padding: "3px 10px", borderRadius: 12,
                background: selectedTags.includes(t.value) ? "var(--burgundy)" : "var(--surface)",
                color: selectedTags.includes(t.value) ? "white" : "black",
                border: "none", cursor: "pointer",
              }}
            >
              {t.value} ({t.count})
            </button>
          ))}
        </div>
      )}

      {loading ? (
        <p>Učitavanje...</p>
      ) : resources.length === 0 ? (
        <p>Nema rezultata.</p>
      ) : (
        <div style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fill, minmax(240px, 1fr))",
          gap: 16,
        }}>
          {resources.map((r) => (
            <ResourceCard key={r.id} resource={r} />
          ))}
        </div>
      )}
    </div>
  );
}
