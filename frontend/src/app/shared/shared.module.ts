import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StarRatingComponent } from './components/star-rating/star-rating.component';

@NgModule({
  declarations: [StarRatingComponent],
  imports: [CommonModule],
  exports: [StarRatingComponent],
})
export class SharedModule {}
