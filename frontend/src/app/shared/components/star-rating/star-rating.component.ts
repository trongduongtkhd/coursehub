import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-star-rating',
  templateUrl: './star-rating.component.html',
  styleUrls: ['./star-rating.component.scss'],
})
export class StarRatingComponent {
  @Input() rating = 0;
  @Input() readonly = false;
  @Output() ratingChange = new EventEmitter<number>();

  hoveredStar = 0;
  stars = [1, 2, 3, 4, 5];

  onStarClick(star: number): void {
    if (this.readonly) return;
    this.rating = star;
    this.ratingChange.emit(star);
  }

  onStarHover(star: number): void {
    if (this.readonly) return;
    this.hoveredStar = star;
  }

  onMouseLeave(): void {
    this.hoveredStar = 0;
  }

  isFilled(star: number): boolean {
    return star <= (this.hoveredStar || this.rating);
  }
}
