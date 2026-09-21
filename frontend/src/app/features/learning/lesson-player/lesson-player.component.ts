import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LessonProgress } from 'src/app/core/models/lesson-progress.model';
import { EnrollmentService } from 'src/app/core/services/enrollment.service';

@Component({
  selector: 'app-lesson-player',
  templateUrl: './lesson-player.component.html',
  styleUrls: ['./lesson-player.component.scss'],
})
export class LessonPlayerComponent implements OnInit {
  courseId!: number;
  lessons: LessonProgress[] = [];
  selectedLesson: LessonProgress | null = null;
  loading = true;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private enrollmentService: EnrollmentService,
  ) {}

  ngOnInit(): void {
    this.courseId = Number(this.route.snapshot.paramMap.get('courseId'));
    this.enrollmentService.getCourseProgress(this.courseId).subscribe({
      next: (data) => {
        this.lessons = data;
        this.loading = false;
        if (data.length > 0) {
          this.selectedLesson = data[0];
        }
      },
      error: (err) => {
        this.errorMessage =
          err.error?.message ?? 'Không tải được danh sách bài học.';
        this.loading = false;
      },
    });
  }

  selectLesson(lesson: LessonProgress): void {
    this.selectedLesson = lesson;
  }

  toggleComplete(lesson: LessonProgress, event: Event): void {
    event.stopPropagation();
    const previousState = lesson.isCompleted;
    lesson.isCompleted = !previousState;

    const action = previousState
      ? this.enrollmentService.unmarkComplete(lesson.lessonId)
      : this.enrollmentService.markComplete(lesson.lessonId);

    action.subscribe({
      next: () => {},
      error: (err) => {
        lesson.isCompleted = previousState;
        this.errorMessage = err.error?.message ?? 'Cập nhật thất bại.';
      },
    });
  }

  get completedCount(): number {
    return this.lessons.filter((l) => l.isCompleted).length;
  }

  get progressPercent(): number {
    return this.lessons.length === 0
      ? 0
      : Math.round((this.completedCount / this.lessons.length) * 100);
  }
}
