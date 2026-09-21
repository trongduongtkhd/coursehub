import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Enrollment } from '../models/enrollment.model';

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
}
