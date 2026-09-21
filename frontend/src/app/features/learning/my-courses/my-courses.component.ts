import { Component, OnInit } from '@angular/core';
import { Enrollment } from 'src/app/core/models/enrollment.model';
import { EnrollmentService } from 'src/app/core/services/enrollment.service';

@Component({
  selector: 'app-my-courses',
  templateUrl: './my-courses.component.html',
  styleUrls: ['./my-courses.component.scss'],
})
export class MyCoursesComponent implements OnInit {
  enrollments: Enrollment[] = [];
  loading = true;

  constructor(private enrollmentService: EnrollmentService) {}

  ngOnInit(): void {
    this.enrollmentService.getMyEnrollments().subscribe({
      next: (data) => {
        this.enrollments = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }
}
