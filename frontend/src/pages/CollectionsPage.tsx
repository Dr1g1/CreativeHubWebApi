import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getMyCollections, createCollection, deleteCollection } from "../api/collections";
import type { Collection } from "../types";

export default function CollectionsPage() {
  const [collections, setCollections] = useState<Collection[]>([]);
  const [loading, setLoading] = useState(true);

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [isPublic, setIsPublic] = useState(false);
  const [creating, setCreating] = useState(false);

  function load() {
    getMyCollections()
      .then(setCollections)
      .catch(() => {})
      .finally(() => setLoading(false));
  }

  useEffect(() => {
    load();
  }, []);

  async function handleCreate(e: React.FormEvent) {
    e.preventDefault();
    if (!name.trim()) return;
    setCreating(true);
    try {
      await createCollection({ name, description, isPublic });
      setName("");
      setDescription("");
      setIsPublic(false);
      load();
    } finally {
      setCreating(false);
    }
  }

  async function handleDelete(id: string) {
    await deleteCollection(id);
    load();
  }

  const inputStyle = { padding: 8, borderRadius: 8, border: "1px solid #ccc", fontSize: 14 };

  return (
    <div style={{ padding: 20, maxWidth: 720, margin: "0 auto" }}>
      <h1>Moje kolekcije</h1>

      <form onSubmit={handleCreate}
        style={{ display: "grid", gap: 8, maxWidth: 420, margin: "12px 0 28px",
                 padding: 16, border: "1px solid #eee", borderRadius: 8 }}>
        <strong>Nova kolekcija</strong>
        <input placeholder="Naziv" value={name}
          onChange={(e) => setName(e.target.value)} required style={inputStyle} />
        <textarea placeholder="Opis (opciono)" value={description}
          onChange={(e) => setDescription(e.target.value)} rows={2} style={inputStyle} />
        <label style={{ display: "flex", gap: 6, alignItems: "center", fontSize: 14 }}>
          <input type="checkbox" checked={isPublic}
            onChange={(e) => setIsPublic(e.target.checked)} />
          Javna (vidljiva drugima)
        </label>
        <button type="submit" disabled={creating}
          style={{ ...inputStyle, background: "var(--burgundy)", color: "white", cursor: "pointer" }}>
          {creating ? "Pravljenje..." : "Napravi"}
        </button>
      </form>

      {loading ? (
        <p>Učitavanje...</p>
      ) : collections.length === 0 ? (
        <p style={{ color: "#666" }}>Još nemaš kolekcije.</p>
      ) : (
        <div style={{ display: "grid", gap: 10 }}>
          {collections.map((c) => (
            <div key={c.id}
              style={{ display: "flex", alignItems: "center", gap: 12,
                       padding: 12, border: "1px solid #eee", borderRadius: 8 }}>
              <div style={{ flex: 1 }}>
                <Link to={`/collections/${c.id}`} style={{ fontWeight: "bold" }}>
                  {c.name}
                </Link>
                <p style={{ margin: "2px 0 0", fontSize: 13, color: "#888" }}>
                  {c.resourceIds.length} resursa · {c.isPublic ? "javna" : "privatna"}
                </p>
              </div>
              <button onClick={() => handleDelete(c.id)}
                style={{ ...inputStyle, cursor: "pointer" }}>
                Obriši
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
