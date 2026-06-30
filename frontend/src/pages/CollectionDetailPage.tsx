import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getCollection, removeResourceFromCollection } from "../api/collections";
import { getResource } from "../api/resources";
import type { Collection, Resource } from "../types";
import { useAuth } from "../context/AuthContext";
import ResourceCard from "../components/ResourceCard";

export default function CollectionDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { userId } = useAuth();

  const [collection, setCollection] = useState<Collection | null>(null);
  const [resources, setResources] = useState<Resource[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  function load(collectionId: string) {
    getCollection(collectionId)
      .then(async (col) => {
        setCollection(col);
        const fetched = await Promise.all(
          col.resourceIds.map((rid) => getResource(rid).catch(() => null))
        );
        setResources(fetched.filter((r): r is Resource => r !== null));
      })
      .catch(() => setError("Kolekcija nije pronađena."))
      .finally(() => setLoading(false));
  }

  useEffect(() => {
    if (!id) return;
    load(id);
  }, [id]);

  async function handleRemove(resourceId: string) {
    if (!id) return;
    await removeResourceFromCollection(id, resourceId);
    load(id); 
  }

  if (loading) return <p style={{ padding: 20 }}>Učitavanje...</p>;
  if (error || !collection) return <p style={{ padding: 20, color: "red" }}>{error}</p>;

  const isOwner = collection.ownerId === userId;

  return (
    <div style={{ padding: 20, maxWidth: 900, margin: "0 auto" }}>
      <h1>{collection.name}</h1>
      {collection.description && <p style={{ color: "#666" }}>{collection.description}</p>}
      <p style={{ fontSize: 13, color: "#888" }}>
        {collection.resourceIds.length} resursa · {collection.isPublic ? "javna" : "privatna"}
      </p>

      {resources.length === 0 ? (
        <p style={{ color: "#666", marginTop: 16 }}>Kolekcija je prazna.</p>
      ) : (
        <div style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fill, minmax(240px, 1fr))",
          gap: 16, marginTop: 16,
        }}>
          {resources.map((r) => (
            <div key={r.id} style={{ position: "relative" }}>
              <ResourceCard resource={r} />
              {isOwner && (
                <button onClick={() => handleRemove(r.id)}
                  style={{ position: "absolute", top: 8, right: 8,
                           borderRadius: 8, border: "1px solid #ccc",
                           background: "white", cursor: "pointer", padding: "4px 8px" }}>
                  Izbaci
                </button>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
