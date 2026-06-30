export interface AuthResponse {
  token: string;
  id: string;
  username: string;
  displayName: string;
}

export interface Resource {
  id: string;
  title: string;
  description: string;
  type: string; //cetke, teksture, 3D modeli, colorpalete i templejtovi  
  tags: string[];
  colors: string[];
  fileFormat: string;
  fileSizeBytes: number;
  ownerId: string;
  downloads: number;
  averageRating: number;
  reviewCount: number;
  createdAt: string;
  previewImageIds: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface FacetCount {
  value: string;
  count: number;
}

export interface FacetResult {
  byType: FacetCount[];
  topTags: FacetCount[];
}

export interface SearchQuery {
  text?: string;
  type?: string;
  tags?: string[];
  sortBy?: string;
  page?: number;
  pageSize?: number;
  excludeOwnerId?: string;
}

export interface UserProfile {
  id: string;
  username: string;
  email: string;
  displayName: string;
  bio: string;
}

export interface Review {
  id: string;
  resourceId: string;
  userId: string;
  rating: number;     
  comment: string;
  createdAt: string;
}

export interface CreateReview {
  rating: number;
  comment: string;
}

export interface Collection {
  id: string;
  name: string;
  description: string;
  ownerId: string;
  resourceIds: string[];
  isPublic: boolean;
}

export interface CreateCollection {
  name: string;
  description: string;
  isPublic: boolean;
}

export interface TopCreator {
  creatorId: string;
  username: string;        
  resourceCount: number;
  totalDownloads: number;
}

export interface RatingBucket {
  range: string;   
  count: number;   
}