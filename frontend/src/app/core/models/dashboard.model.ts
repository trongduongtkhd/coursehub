export interface MonthlyEnrollment {
  year: number;
  month: number;
  count: number;
}

export interface TopCourse {
  courseId: number;
  title: string;
  enrollmentCount: number;
}

export interface DashboardStats {
  totalUsers: number;
  totalAdmins: number;
  totalInstructors: number;
  totalStudents: number;
  totalCourses: number;
  draftCourses: number;
  publishedCourses: number;
  archivedCourses: number;
  totalEnrollments: number;
  averageRatingSystemWide: number;
  enrollmentsByMonth: MonthlyEnrollment[];
  topCourses: TopCourse[];
}
