import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  getResource, downloadUrl, previewUrl,
  updateResource, addPreview, removePreview,
} from "../api/resources";
import { getReviews, addReview, updateReview, deleteReview } from "../api/reviews";
import type { Resource, Review } from "../types";
import { useAuth } from "../context/AuthContext";
import StarRating from "../components/StarRating";
import AddToCollection from "../components/AddToCollection";

export default function ResourceDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { isLoggedIn, userId } = useAuth();

  const [resource, setResource] = useState<Resource | null>(null);
  const [reviews, setReviews] = useState<Review[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  
  const [rating, setRating] = useState(5);
  const [comment, setComment] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [reviewError, setReviewError] = useState("");

  
  const [editing, setEditing] = useState(false);
  const [editTitle, setEditTitle] = useState("");
  const [editDescription, setEditDescription] = useState("");
  const [editTags, setEditTags] = useState("");
  const [editColors, setEditColors] = useState<string[]>([]);
  const [savingEdit, setSavingEdit] = useState(false);

  const myReview = reviews.find((r) => r.userId === userId) ?? null;
  const isOwner = !!resource && resource.ownerId === userId;

  function loadAll(resourceId: string) {
    Promise.all([getResource(resourceId), getReviews(resourceId)])
      .then(([res, revs]) => {
        setResource(res);
        setReviews(revs);
        const mine = revs.find((r) => r.userId === userId);
        if (mine) {
          setRating(mine.rating);
          setComment(mine.comment);
        }
      })
      .catch(() => setError("Resurs nije pronađen."))
      .finally(() => setLoading(false));
  }

  useEffect(() => {
    if (!id) return;
    loadAll(id);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id, userId]);

  function startEditing() {
    if (!resource) return;
    setEditTitle(resource.title);
    setEditDescription(resource.description);
    setEditTags(resource.tags.join(", "));
    setEditColors(resource.colors);
    setEditing(true);
  }

  async function handleSaveEdit() {
    if (!id) return;
    setSavingEdit(true);
    try {
      await updateResource(id, {
        title: editTitle,
        description: editDescription,
        tags: editTags.split(",").map((t) => t.trim()).filter(Boolean),
        colors: editColors,
      });
      setEditing(false);
      loadAll(id);
    } finally {
      setSavingEdit(false);
    }
  }

  async function handleAddPreview(e: React.ChangeEvent<HTMLInputElement>) {
    if (!id || !e.target.files?.[0]) return;
    await addPreview(id, e.target.files[0]);
    e.target.value = ""; 
    loadAll(id);
  }

  async function handleRemovePreview(fileId: string) {
    if (!id) return;
    await removePreview(id, fileId);
    loadAll(id);
  }

  async function handleSubmitReview(e: React.FormEvent) {
    e.preventDefault();
    if (!id) return;
    setReviewError("");
    setSubmitting(true);
    try {
      if (myReview) await updateReview(id, myReview.id, { rating, comment });
      else await addReview(id, { rating, comment });
      loadAll(id);
    } catch {
      setReviewError("Nije uspelo slanje recenzije.");
    } finally {
      setSubmitting(false);
    }
  }

  async function handleDeleteReview() {
    if (!id || !myReview) return;
    await deleteReview(id, myReview.id);
    setRating(5);
    setComment("");
    loadAll(id);
  }

  function updateEditColor(i: number, value: string) {
    setEditColors((prev) => prev.map((c, idx) => (idx === i ? value : c)));
  }
  function addEditColor() {
    setEditColors((prev) => [...prev, "#000000"]);
  }
  function removeEditColor(i: number) {
    setEditColors((prev) => prev.filter((_, idx) => idx !== i));
  }

  if (loading) return <p style={{ padding: 20 }}>Učitavanje...</p>;
  if (error || !resource) return <p style={{ padding: 20, color: "red" }}>{error}</p>;

  const inputStyle = { padding: 8, borderRadius: 8, border: "1px solid var(--border)", fontSize: 14 };

  return (
    <div style={{ padding: 20, maxWidth: 720, margin: "0 auto" }}>

      {!editing ? (
        <>
          <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
            <h1 style={{ margin: 0 }}>{resource.title}</h1>
            {isOwner && (
              <button onClick={startEditing}
                style={{ background: "var(--surface)", color: "var(--text)" }}>
                Izmeni
              </button>
            )}
          </div>
          <p style={{ color: "#666" }}>
            {resource.type} · {resource.fileFormat || "paleta"} ·{" "}
            {resource.downloads} preuzimanja · {resource.averageRating.toFixed(1)}/5 ({resource.reviewCount})
          </p>
          <p>{resource.description}</p>

          {resource.colors.length > 0 && (
            <div style={{ display: "flex", gap: 6, margin: "12px 0" }}>
              {resource.colors.map((c) => (
                <div key={c} title={c}
                  style={{ width: 40, height: 40, borderRadius: 6, background: c }} />
              ))}
            </div>
          )}

          <div style={{ display: "flex", gap: 6, flexWrap: "wrap", margin: "12px 0" }}>
            {resource.tags.map((t) => (
              <span key={t} style={{ fontSize: 13, background: "var(--surface)",
                                     padding: "3px 10px", borderRadius: 12 }}>
                {t}
              </span>
            ))}
          </div>
        </>
      ) : (
        <div style={{ padding: 16, border: "1px solid var(--burgundy)", borderRadius: 8,
                      background: "white", display: "grid", gap: 10, marginBottom: 16 }}>
          <strong>Izmena resursa</strong>
          <input value={editTitle} onChange={(e) => setEditTitle(e.target.value)}
            placeholder="Naslov" style={inputStyle} />
          <textarea value={editDescription} onChange={(e) => setEditDescription(e.target.value)}
            placeholder="Opis" rows={3} style={inputStyle} />
          <input value={editTags} onChange={(e) => setEditTags(e.target.value)}
            placeholder="Tagovi (odvojeni zarezom)" style={inputStyle} />

          {resource.colors.length > 0 && (
            <div>
              <p style={{ margin: "0 0 6px" }}>Boje:</p>
              <div style={{ display: "grid", gap: 6 }}>
                {editColors.map((c, i) => (
                  <div key={i} style={{ display: "flex", gap: 8, alignItems: "center" }}>
                    <input type="color" value={c} onChange={(e) => updateEditColor(i, e.target.value)} />
                    <span style={{ fontSize: 13 }}>{c}</span>
                    {editColors.length > 1 && (
                      <button type="button" onClick={() => removeEditColor(i)}
                        style={{ background: "var(--surface)", color: "var(--text)" }}>
                        Ukloni
                      </button>
                    )}
                  </div>
                ))}
              </div>
              <button type="button" onClick={addEditColor}
                style={{ marginTop: 6, background: "var(--surface)", color: "var(--text)" }}>
                + Dodaj boju
              </button>
            </div>
          )}

          <div style={{ display: "flex", gap: 8 }}>
            <button onClick={handleSaveEdit} disabled={savingEdit}
              style={{ background: "var(--burgundy)", color: "white" }}>
              {savingEdit ? "Čuvanje..." : "Sačuvaj"}
            </button>
            <button onClick={() => setEditing(false)} disabled={savingEdit}
              style={{ background: "var(--surface)", color: "var(--text)" }}>
              Otkaži
            </button>
          </div>
        </div>
      )}

      {(resource.previewImageIds.length > 0 || isOwner) && (
        <div style={{ margin: "16px 0" }}>
          <div style={{ display: "flex", gap: 8, flexWrap: "wrap" }}>
            {resource.previewImageIds.map((pid) => (
              <div key={pid} style={{ position: "relative" }}>
                <img src={previewUrl(pid)} alt="preview"
                  style={{ width: 180, height: 180, objectFit: "cover", borderRadius: 8 }} />
                {isOwner && (
                  <button onClick={() => handleRemovePreview(pid)}
                    style={{ position: "absolute", top: 6, right: 6, padding: "2px 8px",
                             background: "var(--burgundy-dark)", color: "white" }}>
                    Obriši
                  </button>
                )}
              </div>
            ))}
          </div>
          {isOwner && (
            <label style={{ display: "inline-block", marginTop: 10 }}>
              Dodaj preview sliku:
              <input type="file" accept="image/*" onChange={handleAddPreview}
                style={{ display: "block", marginTop: 4 }} />
            </label>
          )}
        </div>
      )}

      {resource.fileFormat && (
        <a href={downloadUrl(resource.id)}
           style={{ display: "inline-block", marginTop: 8, padding: "8px 16px",
                    background: "var(--burgundy)", color: "white", borderRadius: 8,
                    textDecoration: "none" }}>
          Skini fajl
        </a>
      )}
      {isLoggedIn && <AddToCollection resourceId={resource.id} />}

      <hr style={{ margin: "28px 0", border: "none", borderTop: "1px solid var(--border)" }} />
      <h2>Recenzije ({reviews.length})</h2>

      {isLoggedIn && !isOwner && (
        <form onSubmit={handleSubmitReview}
          style={{ display: "grid", gap: 10, maxWidth: 480, margin: "12px 0 24px",
                   padding: 16, border: "1px solid var(--border)", borderRadius: 8, background: "white" }}>
          <strong>{myReview ? "Izmeni svoju recenziju" : "Ostavi recenziju"}</strong>
          <StarRating value={rating} onChange={setRating} />
          <textarea placeholder="Komentar..." value={comment}
            onChange={(e) => setComment(e.target.value)} rows={3} style={inputStyle} />
          <div style={{ display: "flex", gap: 8 }}>
            <button type="submit" disabled={submitting}
              style={{ background: "var(--burgundy)", color: "white" }}>
              {submitting ? "Slanje..." : myReview ? "Sačuvaj izmenu" : "Pošalji"}
            </button>
            {myReview && (
              <button type="button" onClick={handleDeleteReview}
                style={{ background: "var(--surface)", color: "var(--text)" }}>
                Obriši
              </button>
            )}
          </div>
          {reviewError && <span style={{ color: "red" }}>{reviewError}</span>}
        </form>
      )}

      {isLoggedIn && isOwner && (
        <p style={{ color: "#666" }}>Ovo je tvoj resurs — ne možeš ga oceniti.</p>
      )}
      {!isLoggedIn && (
        <p style={{ color: "#666" }}>Prijavi se da bi ostavila recenziju.</p>
      )}

      {reviews.length === 0 ? (
        <p style={{ color: "#666" }}>Još nema recenzija.</p>
      ) : (
        <div style={{ display: "grid", gap: 12 }}>
          {reviews.map((r) => (
            <div key={r.id}
              style={{ padding: 12, border: "1px solid var(--border)", borderRadius: 8, background: "white" }}>
              <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
                <StarRating value={r.rating} size={16} />
                {r.userId === userId && (
                  <span style={{ fontSize: 12, color: "#888" }}>(ti)</span>
                )}
              </div>
              {r.comment && <p style={{ margin: "6px 0 0" }}>{r.comment}</p>}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
