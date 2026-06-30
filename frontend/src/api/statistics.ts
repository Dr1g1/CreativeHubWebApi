import client from "./client";
import type { TopCreator, RatingBucket } from "../types";

export async function getTopCreators() {
  const res = await client.get<TopCreator[]>("/statistics/top-creators");
  return res.data;
}

export async function getRatingDistribution() {
  const res = await client.get<RatingBucket[]>("/statistics/rating-distribution");
  return res.data;
}