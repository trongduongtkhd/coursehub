import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Enrollment } from '../models/enrollment.model';
import { LessonProgress } from '../models/lesson-progress.model';

@Injectable({ providedIn: 'root' })
export class EnrollmentService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  enroll(courseId: number): Observable<Enrollment> {
    return this.http.post<Enrollment>(
      `${this.baseUrl}/courses/${courseId}/enroll`,
      {},
    );
  }

  getMyEnrollments(): Observable<Enrollment[]> {
    return this.http.get<Enrollment[]>(`${this.baseUrl}/enrollments/my`);
  }

  getCourseProgress(courseId: number): Observable<LessonProgress[]> {
    return this.http.get<LessonProgress[]>(
      `${this.baseUrl}/courses/${courseId}/my-progress`,
    );
  }

  markComplete(lessonId: number): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}/lessons/${lessonId}/complete`,
      {},
    );
  }

  unmarkComplete(lessonId: number): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}/lessons/${lessonId}/complete`,
    );
  }
}
