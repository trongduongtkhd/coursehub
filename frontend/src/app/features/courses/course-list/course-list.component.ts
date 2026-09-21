import { Component, OnInit } from '@angular/core';
import { CourseService } from 'src/app/core/services/course.service';
import { Course } from 'src/app/core/models/course.model';
import { AuthService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-course-list',
  templateUrl: './course-list.component.html',
  styleUrls: ['./course-list.component.scss'],
})
export class CourseListComponent implements OnInit {
  courses: Course[] = [];
  loading = true;

  constructor(
    private courseService: CourseService,
    private authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.courseService.getAll().subscribe({
      next: (data) => {
        this.courses = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  get canCreate(): boolean {
    const role = this.authService.getRole();
    return role === 'Admin' || role === 'Instructor';
  }
}
