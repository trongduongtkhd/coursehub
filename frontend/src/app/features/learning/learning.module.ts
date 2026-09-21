import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LearningRoutingModule } from './learning-routing.module';
import { MyCoursesComponent } from './my-courses/my-courses.component';


@NgModule({
  declarations: [
    MyCoursesComponent
  ],
  imports: [
    CommonModule,
    LearningRoutingModule
  ]
})
export class LearningModule { }
