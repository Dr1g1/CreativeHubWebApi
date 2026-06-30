import client from "./client";
import type { UserProfile, Resource } from "../types";

export async function getMyProfile() {
  const res = await client.get<UserProfile>("/users/me");
  return res.data;
}

export async function updateMyProfile(displayName: string, bio: string) {
  const res = await client.put<UserProfile>("/users/me", { displayName, bio });
  return res.data;
}

export async function getMyResources() {
  const res = await client.get<Resource[]>("/users/me/resources");
  return res.data;
}

export async function deleteMyAccount() {
  await client.delete("/users/me");
}