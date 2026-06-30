import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getResource, downloadUrl, previewUrl } from "../api/resources";
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

  const myReview = reviews.find((r) => r.userId === userId) ?? null;

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

  async function handleSubmitReview(e: React.FormEvent) {
    e.preventDefault();
    if (!id) return;
    setReviewError("");
    setSubmitting(true);
    try {
      if (myReview) {
        await updateReview(id, myReview.id, { rating, comment });
      } else {
        await addReview(id, { rating, comment });
      }
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

  if (loading) return <p style={{ padding: 20 }}>Učitavanje...</p>;
  if (error || !resource) return <p style={{ padding: 20, color: "red" }}>{error}</p>;

  const inputStyle = { padding: 8, borderRadius: 8, border: "1px solid #ccc", fontSize: 14 };

  return (
    <div style={{ padding: 20, maxWidth: 720, margin: "0 auto" }}>
      <h1>{resource.title}</h1>
      <p style={{ color: "#666" }}>
        {resource.type} · {resource.fileFormat || "paleta"} · ⬇ {resource.downloads} ·{" "}
        ★ {resource.averageRating.toFixed(1)} ({resource.reviewCount})
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

      {resource.previewImageIds.length > 0 && (
        <div style={{ display: "flex", gap: 8, flexWrap: "wrap", margin: "16px 0" }}>
          {resource.previewImageIds.map((pid) => (
            <img key={pid} src={previewUrl(pid)} alt="preview"
              style={{ width: 180, height: 180, objectFit: "cover", borderRadius: 8 }} />
          ))}
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

      <hr style={{ margin: "28px 0", border: "none", borderTop: "1px solid #eee" }} />
      <h2>Recenzije ({reviews.length})</h2>

      {isLoggedIn && resource.ownerId !== userId && (
        <form onSubmit={handleSubmitReview}
          style={{ display: "grid", gap: 10, maxWidth: 480, margin: "12px 0 24px",
                   padding: 16, border: "1px solid #eee", borderRadius: 8 }}>
          <strong>{myReview ? "Izmeni svoju recenziju" : "Ostavi recenziju"}</strong>
          <StarRating value={rating} onChange={setRating} />
          <textarea placeholder="Komentar..." value={comment}
            onChange={(e) => setComment(e.target.value)} rows={3} style={inputStyle} />
          <div style={{ display: "flex", gap: 8 }}>
            <button type="submit" disabled={submitting}
              style={{ ...inputStyle, background: "var(--burgundy)", color: "white", cursor: "pointer" }}>
              {submitting ? "Slanje..." : myReview ? "Sačuvaj izmenu" : "Pošalji"}
            </button>
            {myReview && (
              <button type="button" onClick={handleDeleteReview}
                style={{ ...inputStyle, cursor: "pointer" }}>
                Obriši
              </button>
            )}
          </div>
          {reviewError && <span style={{ color: "red" }}>{reviewError}</span>}
        </form>
      )}

      {isLoggedIn && resource.ownerId === userId && (
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
              style={{ padding: 12, border: "1px solid #eee", borderRadius: 8 }}>
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
