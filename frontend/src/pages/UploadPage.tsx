import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { uploadResource, createPalette } from "../api/resources";

export default function UploadPage() {
  const navigate = useNavigate();
  const [mode, setMode] = useState<"file" | "palette">("file");

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [tagsText, setTagsText] = useState("");
  const [type, setType] = useState("Brush");
  const [file, setFile] = useState<File | null>(null);
  const [previews, setPreviews] = useState<File[]>([]);
  const [colors, setColors] = useState<string[]>(["#ff5733"]);

  const [error, setError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  function parseTags() {
    return tagsText.split(",").map((t) => t.trim()).filter(Boolean);
  }

  function updateColor(i: number, value: string) {
    setColors((prev) => prev.map((c, idx) => (idx === i ? value : c)));
  }
  function addColor() {
    setColors((prev) => [...prev, "#000000"]);
  }
  function removeColor(i: number) {
    setColors((prev) => prev.filter((_, idx) => idx !== i));
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    setSubmitting(true);
    try {
      if (mode === "file") {
        if (!file) {
          setError("Izaberi glavni fajl.");
          setSubmitting(false);
          return;
        }
        const created = await uploadResource({
          title, description, type, tags: parseTags(), file, previews,
        });
        navigate(`/resources/${created.id}`);
      } else {
        const created = await createPalette({
            title, description, tags: parseTags(), colors, previews,
        });
        navigate(`/resources/${created.id}`);
      }
    } catch {
      setError("Postavljanje nije uspelo.");
      setSubmitting(false);
    }
  }

  const inputStyle = { padding: 8, borderRadius: 8, border: "1px solid #ccc", fontSize: 14 };

  return (
    <div style={{ maxWidth: 560, margin: "30px auto", padding: 20 }}>
      <h1>Postavi resurs</h1>

      <div style={{ display: "flex", gap: 8, marginBottom: 20 }}>
        <button
          onClick={() => setMode("file")}
          style={{ ...inputStyle, fontWeight: mode === "file" ? "bold" : "normal", cursor: "pointer" }}
        >
          Fajl (četka, tekstura, 3D...)
        </button>
        <button
          onClick={() => setMode("palette")}
          style={{ ...inputStyle, fontWeight: mode === "palette" ? "bold" : "normal", cursor: "pointer" }}
        >
          Paleta boja
        </button>
      </div>

      <form onSubmit={handleSubmit} style={{ display: "grid", gap: 12 }}>
        <input placeholder="Naslov" value={title}
          onChange={(e) => setTitle(e.target.value)} required style={inputStyle} />

        <textarea placeholder="Opis" value={description}
          onChange={(e) => setDescription(e.target.value)} rows={3} style={inputStyle} />

        <input placeholder="Tagovi (odvojeni zarezom): fantasy, brush, tree"
          value={tagsText} onChange={(e) => setTagsText(e.target.value)} style={inputStyle} />

        {mode === "file" ? (
          <>
            <label>
              Tip:{" "}
              <select value={type} onChange={(e) => setType(e.target.value)} style={inputStyle}>
                <option value="Brush">Brush</option>
                <option value="Texture">Texture</option>
                <option value="Model3D">Model3D</option>
                <option value="Template">Template</option>
              </select>
            </label>

            <label>
              Glavni fajl:
              <input type="file"
                onChange={(e) => setFile(e.target.files?.[0] ?? null)}
                style={{ display: "block", marginTop: 4 }} />
            </label>

            <label>
              Preview slike (možeš izabrati više):
              <input type="file" multiple accept="image/*"
                onChange={(e) => setPreviews(Array.from(e.target.files ?? []))}
                style={{ display: "block", marginTop: 4 }} />
            </label>

            {previews.length > 0 && (
              <p style={{ fontSize: 13, color: "#666" }}>
                Izabrano slika: {previews.length}
              </p>
            )}
          </>
        ) : (
          <div>
            <p style={{ marginBottom: 8 }}>Boje:</p>
            <div style={{ display: "grid", gap: 8 }}>
              {colors.map((c, i) => (
                <div key={i} style={{ display: "flex", gap: 8, alignItems: "center" }}>
                  <input type="color" value={c}
                    onChange={(e) => updateColor(i, e.target.value)} />
                  <span style={{ fontSize: 14 }}>{c}</span>
                  {colors.length > 1 && (
                    <button type="button" onClick={() => removeColor(i)}
                      style={{ ...inputStyle, cursor: "pointer" }}>
                      Ukloni
                    </button>
                  )}
                </div>
              ))}
            </div>
            <button type="button" onClick={addColor}
              style={{ ...inputStyle, marginTop: 8, cursor: "pointer" }}>
              + Dodaj boju
            </button>
            <label style={{ display: "block", marginTop: 12 }}>
                Preview slike (opciono):
                <input type="file" multiple accept="image/*"
                    onChange={(e) => setPreviews(Array.from(e.target.files ?? []))}
                    style={{ display: "block", marginTop: 4 }} />
            </label>
          </div>
        )}

        <button type="submit" disabled={submitting}
          style={{ ...inputStyle, background: "var(--burgundy)", color: "white", cursor: "pointer" }}>
          {submitting ? "Postavljanje..." : "Postavi"}
        </button>
      </form>

      {error && <p style={{ color: "red", marginTop: 12 }}>{error}</p>}
    </div>
  );
}