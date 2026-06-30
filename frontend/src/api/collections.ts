import client from "./client";
import type { Collection, CreateCollection } from "../types";

export async function getMyCollections() {
  const res = await client.get<Collection[]>("/collections/mine");
  return res.data;
}

export async function getCollection(id: string) {
  const res = await client.get<Collection>(`/collections/${id}`);
  return res.data;
}

export async function createCollection(data: CreateCollection) {
  const res = await client.post<Collection>("/collections", data);
  return res.data;
}

export async function addResourceToCollection(collectionId: string, resourceId: string) {
  await client.post(`/collections/${collectionId}/resources/${resourceId}`);
}

export async function removeResourceFromCollection(collectionId: string, resourceId: string) {
  await client.delete(`/collections/${collectionId}/resources/${resourceId}`);
}

export async function deleteCollection(id: string) {
  await client.delete(`/collections/${id}`);
}