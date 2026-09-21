import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CourseService } from 'src/app/core/services/course.service';
import { ReviewService } from 'src/app/core/services/review.service';
import { Course } from 'src/app/core/models/course.model';
import { AuthService } from 'src/app/core/services/auth.service';
import { EnrollmentService } from 'src/app/core/services/enrollment.service';
import { CourseRatingSummary, Review } from 'src/app/core/models/review.model';

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

  reviews: Review[] = [];
  ratingSummary: CourseRatingSummary | null = null;
  myRating = 0;
  myComment = '';
  submittingReview = false;
  reviewError = '';

  constructor(
    private route: ActivatedRoute,
    private courseService: CourseService,
    private authService: AuthService,
    private enrollmentService: EnrollmentService,
    private reviewService: ReviewService,
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
    this.loadReviews(id);
  }

  loadReviews(courseId: number): void {
    this.reviewService
      .getByCourse(courseId)
      .subscribe((data) => (this.reviews = data));
    this.reviewService
      .getSummary(courseId)
      .subscribe((data) => (this.ratingSummary = data));
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

  get canReview(): boolean {
    return !!this.authService.getCurrentUser();
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

  onRatingChange(star: number): void {
    this.myRating = star;
  }

  onSubmitReview(): void {
    if (!this.course || this.myRating === 0) return;
    this.submittingReview = true;
    this.reviewError = '';

    this.reviewService
      .upsert(this.course.id, {
        rating: this.myRating,
        comment: this.myComment,
      })
      .subscribe({
        next: () => {
          this.submittingReview = false;
          this.myRating = 0;
          this.myComment = '';
          this.loadReviews(this.course!.id);
        },
        error: (err) => {
          this.reviewError = err.error?.message ?? 'Gửi đánh giá thất bại.';
          this.submittingReview = false;
        },
      });
  }
}
