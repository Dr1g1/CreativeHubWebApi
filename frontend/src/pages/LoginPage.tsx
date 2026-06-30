import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { login as loginApi } from "../api/auth";
import { useAuth } from "../context/AuthContext";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const { login } = useAuth();        
  const navigate = useNavigate();     

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();               
    setError("");
    try {
      const data = await loginApi(email, password);
      login(data);                   
      navigate("/");                 
    } catch {
      setError("Pogrešan email ili lozinka.");
    }
  }

  return (
    <div style={{ maxWidth: 360, margin: "60px auto", display: "grid", gap: 8 }}>
      <h1>Prijava</h1>
      <form onSubmit={handleSubmit} style={{ display: "grid", gap: 8 }}>
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Lozinka"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit">Prijavi se</button>
      </form>
      {error && <p style={{ color: "red" }}>{error}</p>}
      <p>
        Nemaš nalog? <Link to="/register">Registruj se</Link>
      </p>
    </div>
  );
}