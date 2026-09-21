import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Lesson } from 'src/app/core/models/lesson.model';
import { LessonService } from 'src/app/core/services/lesson.service';

@Component({
  selector: 'app-lesson-manager',
  templateUrl: './lesson-manager.component.html',
  styleUrls: ['./lesson-manager.component.scss'],
})
export class LessonManagerComponent implements OnInit {
  @Input() courseId!: number;

  lessons: Lesson[] = [];
  loading = true;
  errorMessage = '';
  showAddForm = false;
  addForm: FormGroup;

  constructor(
    private lessonService: LessonService,
    private fb: FormBuilder,
  ) {
    this.addForm = this.fb.group({
      title: ['', Validators.required],
      content: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadLessons();
  }

  loadLessons(): void {
    this.loading = true;
    this.lessonService.getByCourse(this.courseId).subscribe({
      next: (data) => {
        this.lessons = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  onAddLesson(): void {
    if (this.addForm.invalid) {
      return;
    }
    this.lessonService.create(this.courseId, this.addForm.value).subscribe({
      next: () => {
        this.addForm.reset();
        this.showAddForm = false;
        this.loadLessons();
      },
      error: (err) =>
        (this.errorMessage = err.error?.message ?? 'Thêm bài học thất bại.'),
    });
  }

  onDeleteLesson(lesson: Lesson): void {
    if (!confirm(`Xóa bài học "${lesson.title}"?`)) {
      return;
    }
    this.lessonService.delete(lesson.id).subscribe({
      next: () => this.loadLessons(),
      error: (err) =>
        (this.errorMessage = err.error?.message ?? 'Xóa bài học thất bại.'),
    });
  }
}
