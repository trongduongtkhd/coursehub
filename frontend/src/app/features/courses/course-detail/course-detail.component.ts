import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CourseService } from 'src/app/core/services/course.service';
import { Course } from 'src/app/core/models/course.model';
import { AuthService } from 'src/app/core/services/auth.service';
import { EnrollmentService } from 'src/app/core/services/enrollment.service';

@Component({
  selector: 'app-course-detail',
  templateUrl: './course-detail.component.html',
  styleUrls: ['./course-detail.component.scss'],
})
export class CourseDetailComponent implements OnInit {
  course: Course | null = null;
  loading = true;
  notFound = false;

  enrolling = false;
  enrolled = false;
  enrollError = '';

  constructor(
    private route: ActivatedRoute,
    private courseService: CourseService,
    private authService: AuthService,
    private enrollmentService: EnrollmentService,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.courseService.getById(id).subscribe({
      next: (data) => {
        this.course = data;
        this.loading = false;
      },
      error: () => {
        this.notFound = true;
        this.loading = false;
      },
    });
  }

  get canEdit(): boolean {
    if (!this.course) return false;
    const user = this.authService.getCurrentUser();
    if (!user) return false;
    return user.role === 'Admin' || user.id === this.course.instructorId;
  }

  get canEnroll(): boolean {
    if (!this.course || this.enrolled) return false;
    const user = this.authService.getCurrentUser();
    if (!user) return false;
    if (this.course.status !== 'Published') return false;
    return user.id !== this.course.instructorId;
  }

  onEnroll(): void {
    if (!this.course) return;
    this.enrolling = true;
    this.enrollError = '';

    this.enrollmentService.enroll(this.course.id).subscribe({
      next: () => {
        this.enrolled = true;
        this.enrolling = false;
      },
      error: (err) => {
        this.enrollError = err.error?.message ?? 'Đăng ký thất bại.';
        this.enrolling = false;
      },
    });
  }
}
