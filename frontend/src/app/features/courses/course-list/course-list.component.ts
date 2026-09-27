import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CourseService } from 'src/app/core/services/course.service';
import { Course } from 'src/app/core/models/course.model';
import { PagedResult } from 'src/app/core/models/paged-result.model';
import { AuthService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-course-list',
  templateUrl: './course-list.component.html',
  styleUrls: ['./course-list.component.scss'],
})
export class CourseListComponent implements OnInit {
  result: PagedResult<Course> | null = null;
  loading = true;

  searchControl = new FormControl('');
  statusFilter = '';
  currentPage = 1;
  pageSize = 9;

  constructor(
    private courseService: CourseService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
  ) {}

  ngOnInit(): void {
    const params = this.route.snapshot.queryParams;
    this.searchControl.setValue(params['search'] ?? '', { emitEvent: false });
    this.statusFilter = params['status'] ?? '';
    this.currentPage = Number(params['page']) || 1;

    this.load();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.currentPage = 1;
        this.load();
      });
  }

  load(): void {
    this.loading = true;
    this.updateUrl();

    this.courseService
      .getAll({
        search: this.searchControl.value || undefined,
        status: this.statusFilter || undefined,
        page: this.currentPage,
        pageSize: this.pageSize,
      })
      .subscribe({
        next: (data) => {
          this.result = data;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        },
      });
  }

  updateUrl(): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        search: this.searchControl.value || null,
        status: this.statusFilter || null,
        page: this.currentPage > 1 ? this.currentPage : null,
      },
      queryParamsHandling: 'merge',
    });
  }

  onStatusChange(): void {
    this.currentPage = 1;
    this.load();
  }

  goToPage(page: number): void {
    if (!this.result || page < 1 || page > this.result.totalPages) return;
    this.currentPage = page;
    this.load();
  }

  get canCreate(): boolean {
    const role = this.authService.getRole();
    return role === 'Admin' || role === 'Instructor';
  }
}
