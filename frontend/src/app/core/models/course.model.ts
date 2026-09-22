export interface Course {
  id: number;
  title: string;
  description: string;
  thumbnailUrl: string | null;
  status: string;
  instructorId: number;
  instructorName: string;
  createdAt: string;
  averageRating: number;
  reviewCount: number;
}

export interface CreateCourseRequest {
  title: string;
  description: string;
}

export interface UpdateCourseRequest {
  title: string;
  description: string;
  status: string;
}
export interface ThumbnailUploadResponse {
  thumbnailUrl: string;
}
