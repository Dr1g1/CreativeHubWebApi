import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Navbar() {
  const { isLoggedIn, username, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();             
    navigate("/login");    
  }

  return (
    <nav style={{ display: "flex", gap: 16, alignItems: "center",
                  padding: 12, borderBottom: "1px solid #ccc" }}>
      <Link to="/">Početna</Link>
      <div style={{ marginLeft: "auto", display: "flex", gap: 12, alignItems: "center" }}>
        {isLoggedIn ? (
          <>
            <Link to="/upload">Postavi</Link>
            <Link to="/profile">Moj profil</Link>
            <Link to="/collections">Kolekcije</Link>
            <span>Zdravo, {username}</span>
            <Link to="/statistics">Statistika</Link>
            <button onClick={handleLogout}>Odjavi se</button>
          </>
        ) : (
          <>
            <Link to="/login">Prijava</Link>
            <Link to="/register">Registracija</Link>
          </>
        )}
      </div>
    </nav>
  );
}