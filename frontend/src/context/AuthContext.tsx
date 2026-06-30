import { createContext, useContext, useState, type ReactNode } from "react";
import type { AuthResponse } from "../types";

interface AuthState {
  token: string | null;
  username: string | null;
  userId: string | null;
  isLoggedIn: boolean;
  login: (data: AuthResponse) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthState | undefined>(undefined);


export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem("token"));
  const [username, setUsername] = useState<string | null>(() => localStorage.getItem("username"));
  const [userId, setUserId] = useState<string | null>(() => localStorage.getItem("userId"));

  function login(data: AuthResponse) {
    localStorage.setItem("token", data.token);
    localStorage.setItem("username", data.username);
    localStorage.setItem("userId", data.id);
    setToken(data.token);
    setUsername(data.username);
    setUserId(data.id);
  }

  function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("username");
    localStorage.removeItem("userId");
    setToken(null);
    setUsername(null);
    setUserId(null);
  }

  return (
    <AuthContext.Provider
      value={{ token, username, userId, isLoggedIn: !!token, login, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth mora biti unutar AuthProvider-a");
  return ctx;
}