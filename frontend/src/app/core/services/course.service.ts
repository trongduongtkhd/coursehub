import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  Course,
  CreateCourseRequest,
  ThumbnailUploadResponse,
  UpdateCourseRequest,
} from 'src/app/core/models/course.model';
import { PagedResult } from '../models/paged-result.model';

@Injectable({ providedIn: 'root' })
export class CourseService {
  private baseUrl = `${environment.apiUrl}/courses`;

  constructor(private http: HttpClient) {}

  getAll(params: {
    search?: string;
    status?: string;
    page?: number;
    pageSize?: number;
  }): Observable<PagedResult<Course>> {
    let httpParams = new HttpParams();
    if (params.search) httpParams = httpParams.set('search', params.search);
    if (params.status) httpParams = httpParams.set('status', params.status);
    if (params.page)
      httpParams = httpParams.set('page', params.page.toString());
    if (params.pageSize)
      httpParams = httpParams.set('pageSize', params.pageSize.toString());

    return this.http.get<PagedResult<Course>>(this.baseUrl, {
      params: httpParams,
    });
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
