import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { register as registerApi } from "../api/auth";
import { useAuth } from "../context/AuthContext";
import axios from "axios";

export default function RegisterPage() {
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [displayName, setDisplayName] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const { login } = useAuth();
    const navigate = useNavigate();

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    try {
        const data = await registerApi(username, email, password, displayName);
        login(data);          
        navigate("/");
    } catch (err) {
        const message =
        axios.isAxiosError(err) && err.response?.data?.message
        ? err.response.data.message
        : "Registracija nije uspela.";
        setError(message);
    }
  }

  return (
    <div style={{ maxWidth: 360, margin: "60px auto", display: "grid", gap: 8 }}>
        <h1>Registracija</h1>
        <form onSubmit={handleSubmit} style={{ display: "grid", gap: 8 }}>
            <input placeholder="Korisničko ime" value={username}
                onChange={(e) => setUsername(e.target.value)} required />
            <input type="email" placeholder="Email" value={email}
                onChange={(e) => setEmail(e.target.value)} required />
            <input placeholder="Prikazno ime" value={displayName}
                onChange={(e) => setDisplayName(e.target.value)} />
            <input type="password" placeholder="Lozinka" value={password}
                onChange={(e) => setPassword(e.target.value)} required />
            <button type="submit">Registruj se</button>
        </form>
        {error && <p style={{ color: "red" }}>{error}</p>}
        <p>
            Već imaš nalog? <Link to="/login">Prijavi se</Link>
        </p>
    </div>
  );
}