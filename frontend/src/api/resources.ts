import client from "./client";
import type { Resource } from "../types";

export async function getResources() {
  const res = await client.get<Resource[]>("/resources");
  return res.data;
}

export async function getResource(id: string) {
  const res = await client.get<Resource>(`/resources/${id}`);
  return res.data;
}

export function downloadUrl(id: string) {
  return `http://localhost:5158/api/resources/${id}/download`;
}

export async function uploadResource(data: {
  title: string;
  description: string;
  type: string;
  tags: string[];
  file: File;
  previews: File[];
}) {
  const form = new FormData();
  form.append("Title", data.title);
  form.append("Description", data.description);
  form.append("Type", data.type);
  data.tags.forEach((t) => form.append("Tags", t));   
  form.append("file", data.file);
  data.previews.forEach((p) => form.append("previews", p));

  const res = await client.post<Resource>("/resources/upload", form);
  return res.data;
}

export async function createPalette(data: {
  title: string;
  description: string;
  tags: string[];
  colors: string[];
  previews: File[];
}) {
  const form = new FormData();
  form.append("Title", data.title);
  form.append("Description", data.description);
  form.append("Type", "ColorPalette");
  data.tags.forEach((t) => form.append("Tags", t));
  data.colors.forEach((c) => form.append("Colors", c));
  data.previews.forEach((p) => form.append("previews", p));

  const res = await client.post<Resource>("/resources/palette", form);
  return res.data;
}


export function previewUrl(fileId: string) {
  return `http://localhost:5158/api/resources/preview/${fileId}`;
}