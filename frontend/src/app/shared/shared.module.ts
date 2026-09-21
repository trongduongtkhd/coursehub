import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StarRatingComponent } from './components/star-rating/star-rating.component';
import { ApiImagePipe } from './pipes/api-image.pipe';

@NgModule({
  declarations: [StarRatingComponent, ApiImagePipe],
  imports: [CommonModule],
  exports: [StarRatingComponent, ApiImagePipe],
})
export class SharedModule {}
