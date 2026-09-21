import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { RoleGuard } from 'src/app/core/guards/role.guard';
import { CourseListComponent } from './course-list/course-list.component';
import { CourseDetailComponent } from './course-detail/course-detail.component';
import { CourseFormComponent } from './course-form/course-form.component';

const routes: Routes = [
  { path: '', component: CourseListComponent },
  {
    path: 'new',
    component: CourseFormComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Instructor'] },
  },
  {
    path: ':id/edit',
    component: CourseFormComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Instructor'] },
  },
  { path: ':id', component: CourseDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CoursesRoutingModule {}
