import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  CreateLessonRequest,
  Lesson,
  UpdateLessonRequest,
} from 'src/app/core/models/lesson.model';

@Injectable({ providedIn: 'root' })
export class LessonService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getByCourse(courseId: number): Observable<Lesson[]> {
    return this.http.get<Lesson[]>(
      `${this.baseUrl}/courses/${courseId}/lessons`,
    );
  }

  create(courseId: number, request: CreateLessonRequest): Observable<Lesson> {
    return this.http.post<Lesson>(
      `${this.baseUrl}/courses/${courseId}/lessons`,
      request,
    );
  }

  update(lessonId: number, request: UpdateLessonRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/lessons/${lessonId}`, request);
  }

  delete(lessonId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/lessons/${lessonId}`);
  }
}
