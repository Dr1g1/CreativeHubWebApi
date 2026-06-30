import client from "./client";
import type { AuthResponse } from "../types";

export async function login(email: string, password: string) {
  const res = await client.post<AuthResponse>("/auth/login", { email, password });
  return res.data;
}

export async function register(
  username: string,
  email: string,
  password: string,
  displayName: string
) {
  const res = await client.post<AuthResponse>("/auth/register", {
    username,
    email,
    password,
    displayName,
  });
  return res.data;
}