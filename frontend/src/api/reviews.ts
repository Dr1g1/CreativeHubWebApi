import client from "./client";
import type { Review, CreateReview } from "../types";

export async function getReviews(resourceId: string) {
  const res = await client.get<Review[]>(`/resources/${resourceId}/reviews`);
  return res.data;
}

export async function addReview(resourceId: string, data: CreateReview) {
  const res = await client.post<Review>(`/resources/${resourceId}/reviews`, data);
  return res.data;
}

export async function updateReview(resourceId: string, reviewId: string, data: CreateReview) {
  const res = await client.put<Review>(`/resources/${resourceId}/reviews/${reviewId}`, data);
  return res.data;
}

export async function deleteReview(resourceId: string, reviewId: string) {
  await client.delete(`/resources/${resourceId}/reviews/${reviewId}`);
}