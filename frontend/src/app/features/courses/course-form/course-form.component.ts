import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CourseService } from 'src/app/core/services/course.service';

@Component({
  selector: 'app-course-form',
  templateUrl: './course-form.component.html',
  styleUrls: ['./course-form.component.scss'],
})
export class CourseFormComponent implements OnInit {
  form: FormGroup;
  isEditMode = false;
  courseId: number | null = null;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private courseService: CourseService,
    private route: ActivatedRoute,
    private router: Router,
  ) {
    this.form = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      status: ['Draft'],
    });
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode = true;
      this.courseId = Number(idParam);
      this.courseService
        .getById(this.courseId)
        .subscribe((course) => this.form.patchValue(course));
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    if (this.isEditMode && this.courseId) {
      this.courseService.update(this.courseId, this.form.value).subscribe({
        next: () => this.router.navigate(['/courses', this.courseId]),
        error: (err) =>
          (this.errorMessage = err.error?.message ?? 'Cập nhật thất bại.'),
      });
    } else {
      this.courseService.create(this.form.value).subscribe({
        next: (course) => this.router.navigate(['/courses', course.id]),
        error: (err) =>
          (this.errorMessage = err.error?.message ?? 'Tạo khóa học thất bại.'),
      });
    }
  }
}
