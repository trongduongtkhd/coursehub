export interface Lesson {
  id: number;
  title: string;
  content: string;
  orderIndex: number;
  courseId: number;
}

export interface CreateLessonRequest {
  title: string;
  content: string;
}

export interface UpdateLessonRequest {
  title: string;
  content: string;
}
