export interface Enrollment {
  id: number;
  courseId: number;
  courseTitle: string;
  courseThumbnailUrl: string | null;
  instructorName: string;
  enrolledAt: string;
  totalLessons: number;
  completedLessons: number;
  progressPercent: number;
}
