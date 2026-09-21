import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MyCoursesComponent } from './my-courses/my-courses.component';
import { LessonPlayerComponent } from './lesson-player/lesson-player.component';

const routes: Routes = [
  { path: '', component: MyCoursesComponent },
  { path: ':courseId', component: LessonPlayerComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LearningRoutingModule {}
