import client from "./client";
import type { Resource, PagedResult, FacetResult, SearchQuery } from "../types";

export async function searchResources(query: SearchQuery) {
  const res = await client.post<PagedResult<Resource>>("/search", query);
  return res.data;
}

export async function getFacets() {
  const res = await client.get<FacetResult>("/search/facets");
  return res.data;
}