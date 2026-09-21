import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { CoursesRoutingModule } from './courses-routing.module';
import { CourseListComponent } from './course-list/course-list.component';
import { CourseDetailComponent } from './course-detail/course-detail.component';
import { CourseFormComponent } from './course-form/course-form.component';
import { LessonManagerComponent } from './lesson-manager/lesson-manager.component';

@NgModule({
  declarations: [
    CourseListComponent,
    CourseDetailComponent,
    CourseFormComponent,
    LessonManagerComponent,
  ],
  imports: [CommonModule, CoursesRoutingModule, ReactiveFormsModule],
})
export class CoursesModule {}
