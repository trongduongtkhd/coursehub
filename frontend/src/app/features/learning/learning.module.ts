import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LearningRoutingModule } from './learning-routing.module';
import { MyCoursesComponent } from './my-courses/my-courses.component';
import { LessonPlayerComponent } from './lesson-player/lesson-player.component';


@NgModule({
  declarations: [
    MyCoursesComponent,
    LessonPlayerComponent
  ],
  imports: [
    CommonModule,
    LearningRoutingModule
  ]
})
export class LearningModule { }
