import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { getMyProfile, updateMyProfile, getMyResources, deleteMyAccount } from "../api/users";
import { getMyCollections } from "../api/collections";
import type { UserProfile, Resource, Collection } from "../types";
import ResourceCard from "../components/ResourceCard";
import { useAuth } from "../context/AuthContext";

export default function ProfilePage() {
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [resources, setResources] = useState<Resource[]>([]);
  const [collections, setCollections] = useState<Collection[]>([]);
  const [displayName, setDisplayName] = useState("");
  const [bio, setBio] = useState("");
  const [saved, setSaved] = useState(false);
  const [loading, setLoading] = useState(true);

  const navigate = useNavigate();
  const { logout } = useAuth();
  const [confirming, setConfirming] = useState(false);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    Promise.all([getMyProfile(), getMyResources(), getMyCollections()])
      .then(([p, r, c]) => {
        setProfile(p);
        setDisplayName(p.displayName);
        setBio(p.bio);
        setResources(r);
        setCollections(c);
      })
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  async function handleSave(e: React.FormEvent) {
    e.preventDefault();
    const updated = await updateMyProfile(displayName, bio);
    setProfile(updated);
    setSaved(true);
    setTimeout(() => setSaved(false), 2000);
  }

  async function handleDelete() {
    setDeleting(true);
    try {
      await deleteMyAccount();
      logout();              
      navigate("/register"); 
    } catch {
      setDeleting(false);
      setConfirming(false);
    }
  }

  if (loading) return <p style={{ padding: 20 }}>Učitavanje...</p>;
  if (!profile) return <p style={{ padding: 20 }}>Profil nije dostupan.</p>;

  const inputStyle = { padding: 8, borderRadius: 8, border: "1px solid var(--border)", fontSize: 14 };

  return (
    <div style={{ padding: 20, maxWidth: 900, margin: "0 auto" }}>
      <h1>Moj profil</h1>

      <form onSubmit={handleSave}
        style={{ display: "grid", gap: 8, maxWidth: 400, marginBottom: 32 }}>
        <p style={{ color: "#666", margin: 0 }}>
          {profile.username} · {profile.email}
        </p>
        <label>
          Prikazno ime:
          <input value={displayName} onChange={(e) => setDisplayName(e.target.value)}
            style={{ ...inputStyle, display: "block", width: "100%", marginTop: 4 }} />
        </label>
        <label>
          Bio:
          <textarea value={bio} onChange={(e) => setBio(e.target.value)} rows={3}
            style={{ ...inputStyle, display: "block", width: "100%", marginTop: 4 }} />
        </label>
        <button type="submit"
          style={{ ...inputStyle, background: "var(--burgundy)", color: "white", cursor: "pointer" }}>
          Sačuvaj
        </button>
        {saved && <span style={{ color: "green" }}>Sačuvano!</span>}
      </form>

      <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
        <h2 style={{ margin: 0 }}>Moje kolekcije</h2>
        <Link to="/collections" style={{ fontSize: 14 }}>Upravljaj</Link>
      </div>
      {collections.length === 0 ? (
        <p style={{ color: "#666" }}>Nemaš kolekcije.</p>
      ) : (
        <ul>
          {collections.map((c) => (
            <li key={c.id}>
              <Link to={`/collections/${c.id}`}>{c.name}</Link>{" "}
              ({c.resourceIds.length})
            </li>
          ))}
        </ul>
      )}

      <h2 style={{ marginTop: 24 }}>Moji resursi</h2>
      {resources.length === 0 ? (
        <p style={{ color: "#666" }}>Nisi postavila nijedan resurs.</p>
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


      <hr style={{ margin: "40px 0 20px", border: "none", borderTop: "1px solid var(--border)" }} />
      <h2>Brisanje naloga</h2>

      {!confirming ? (
        <button onClick={() => setConfirming(true)}
          style={{ background: "var(--burgundy-dark)", color: "white" }}>
          Obriši nalog
        </button>
      ) : (
        <div style={{ padding: 16, border: "1px solid var(--burgundy)",
                      borderRadius: 8, background: "white", maxWidth: 460 }}>
          <p style={{ marginTop: 0 }}>
            Da li ste sigurni? Ovo trajno briše vaš nalog i <strong>sve vaše resurse,
            recenzije i kolekcije</strong>. Ova radnja se ne može poništiti.
          </p>
          <div style={{ display: "flex", gap: 8 }}>
            <button onClick={handleDelete} disabled={deleting}
              style={{ background: "var(--burgundy-dark)", color: "white" }}>
              {deleting ? "Brisanje..." : "Da, obriši sve"}
            </button>
            <button onClick={() => setConfirming(false)} disabled={deleting}
              style={{ background: "var(--surface)", color: "var(--text)" }}>
              Otkaži
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
