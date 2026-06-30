import { Link } from "react-router-dom";
import type { Resource } from "../types";
import { previewUrl } from "../api/resources";

export default function ResourceCard({ resource }: { resource: Resource }) {
  return (
    <Link
      to={`/resources/${resource.id}`}
      style={{
        border: "1px solid #ddd", borderRadius: 8, padding: 16,
        textDecoration: "none", color: "inherit", display: "block",
      }}
    >
    {resource.previewImageIds.length > 0 && (
        <img
            src={previewUrl(resource.previewImageIds[0])}
            alt={resource.title}
            style={{ width: "100%", height: 140, objectFit: "cover",
                borderRadius: 6, marginBottom: 8 }}
        />
    )}
      <h3 style={{ margin: "0 0 8px" }}>{resource.title}</h3>
      <p style={{ margin: "0 0 8px", color: "#666", fontSize: 14 }}>
        {resource.type} · {resource.fileFormat || "paleta"}
      </p>

      {resource.colors.length > 0 && (
        <div style={{ display: "flex", gap: 4, marginBottom: 8 }}>
          {resource.colors.map((c) => (
            <div key={c} style={{ width: 24, height: 24, borderRadius: 4, background: c }} />
          ))}
        </div>
      )}

      <div style={{ display: "flex", gap: 4, flexWrap: "wrap", marginBottom: 8 }}>
        {resource.tags.map((t) => (
          <span key={t} style={{ fontSize: 12, background: "var(--surface)",
                                  padding: "2px 8px", borderRadius: 12 }}>
            {t}
          </span>
        ))}
      </div>

      <p style={{ margin: 0, fontSize: 13, color: "#888" }}>
        ⬇ {resource.downloads} · ★ {resource.averageRating.toFixed(1)} ({resource.reviewCount})
      </p>
    </Link>
  );
}