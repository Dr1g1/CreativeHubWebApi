import { useEffect, useState } from "react";
import { getMyCollections, addResourceToCollection } from "../api/collections";
import type { Collection } from "../types";

export default function AddToCollection({ resourceId }: { resourceId: string }) {
  const [collections, setCollections] = useState<Collection[]>([]);
  const [selected, setSelected] = useState("");
  const [message, setMessage] = useState("");

  useEffect(() => {
    getMyCollections().then(setCollections).catch(() => {});
  }, []);

  async function handleAdd() {
    if (!selected) return;
    try {
      await addResourceToCollection(selected, resourceId);
      setMessage("Dodato u kolekciju!");
      setTimeout(() => setMessage(""), 2000);
    } catch {
      setMessage("Nije uspelo (možda je već u kolekciji).");
      setTimeout(() => setMessage(""), 2500);
    }
  }

  if (collections.length === 0) return null; 

  const inputStyle = { padding: 8, borderRadius: 8, border: "1px solid #ccc", fontSize: 14 };

  return (
    <div style={{ display: "flex", gap: 8, alignItems: "center", marginTop: 16, flexWrap: "wrap" }}>
      <select value={selected} onChange={(e) => setSelected(e.target.value)} style={inputStyle}>
        <option value="">— izaberi kolekciju —</option>
        {collections.map((c) => (
          <option key={c.id} value={c.id}>{c.name}</option>
        ))}
      </select>
      <button onClick={handleAdd} disabled={!selected}
        style={{ ...inputStyle, cursor: "pointer" }}>
        Dodaj u kolekciju
      </button>
      {message && <span style={{ fontSize: 13, color: "#388e3c" }}>{message}</span>}
    </div>
  );
}
