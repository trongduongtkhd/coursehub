import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  Course,
  CreateCourseRequest,
  ThumbnailUploadResponse,
  UpdateCourseRequest,
} from 'src/app/core/models/course.model';

@Injectable({ providedIn: 'root' })
export class CourseService {
  private baseUrl = `${environment.apiUrl}/courses`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Course[]> {
    return this.http.get<Course[]>(this.baseUrl);
  }

  getById(id: number): Observable<Course> {
    return this.http.get<Course>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateCourseRequest): Observable<Course> {
    return this.http.post<Course>(this.baseUrl, request);
  }

  update(id: number, request: UpdateCourseRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  uploadThumbnail(
    courseId: number,
    file: File,
  ): Observable<ThumbnailUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ThumbnailUploadResponse>(
      `${this.baseUrl}/${courseId}/thumbnail`,
      formData,
    );
  }
}
