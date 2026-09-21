export interface Review {
  id: number;
  courseId: number;
  userId: number;
  userName: string;
  rating: number;
  comment: string | null;
  createdAt: string;
}

export interface CreateReviewRequest {
  rating: number;
  comment?: string;
}

export interface CourseRatingSummary {
  averageRating: number;
  reviewCount: number;
}
