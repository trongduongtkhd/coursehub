import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  CourseRatingSummary,
  CreateReviewRequest,
  Review,
} from 'src/app/core/models/review.model';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getByCourse(courseId: number): Observable<Review[]> {
    return this.http.get<Review[]>(
      `${this.baseUrl}/courses/${courseId}/reviews`,
    );
  }

  getSummary(courseId: number): Observable<CourseRatingSummary> {
    return this.http.get<CourseRatingSummary>(
      `${this.baseUrl}/courses/${courseId}/reviews/summary`,
    );
  }

  upsert(courseId: number, request: CreateReviewRequest): Observable<Review> {
    return this.http.post<Review>(
      `${this.baseUrl}/courses/${courseId}/reviews`,
      request,
    );
  }
}
