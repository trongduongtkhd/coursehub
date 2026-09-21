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
  activeTab: 'info' | 'lessons' = 'info';
  selectedFile: File | null = null;
  previewUrl: string | null = null;
  uploadingThumbnail = false;
  thumbnailError = '';
  currentThumbnailUrl: string | null = null;
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
      this.courseService.getById(this.courseId).subscribe((course) => {
        this.form.patchValue(course);
        this.currentThumbnailUrl = course.thumbnailUrl;
      });
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

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      this.thumbnailError = 'Chỉ chấp nhận file JPG, PNG hoặc WEBP.';
      return;
    }
    if (file.size > 2 * 1024 * 1024) {
      this.thumbnailError = 'Kích thước file tối đa 2MB.';
      return;
    }

    this.thumbnailError = '';
    this.selectedFile = file;

    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl = reader.result as string;
    };
    reader.readAsDataURL(file);
  }

  onUploadThumbnail(): void {
    if (!this.selectedFile || !this.courseId) return;

    this.uploadingThumbnail = true;
    this.thumbnailError = '';

    this.courseService
      .uploadThumbnail(this.courseId, this.selectedFile)
      .subscribe({
        next: (res) => {
          this.currentThumbnailUrl = res.thumbnailUrl;
          this.selectedFile = null;
          this.uploadingThumbnail = false;
        },
        error: (err) => {
          this.thumbnailError = err.error?.message ?? 'Upload thất bại.';
          this.uploadingThumbnail = false;
        },
      });
  }
}
